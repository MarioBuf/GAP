using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Assets.Plugins.GAP.Editor.Users;
using System.Net;
using System.Threading;
using System.ComponentModel;
using Newtonsoft.Json;
using KafkaNet.Model;
using KafkaNet;
using KafkaNet.Protocol;
using Assets.Plugin.GAP.Editor.Users;

namespace Assets.Plugins.GAP.Editor
{
    class avatar
    {
        public string username { get; set; }
        public byte[] avatarUser { get; set; }
        public Texture2D textureAvatar { get; set; }
    }

    public class GUI : EditorWindow, IHasCustomMenu
    {
        //Grafica
        bool foldoutOnline = false;
        bool foldoutOffline = false;
        bool foldoutOnlineControl = true;
        bool foldoutOfflineControl = true;
        public bool ok = true;
        private byte[] imageUserBytes;
        private Texture2D textureLogin;
        private List<avatar> avatarUsers;

        //Informazioni personali
        Info info;
        String username;

        //Altro
        private bool kafkaOk = false;
        List<String> lista = new List<String>();
        Thread thread;
        bool isAlive = true;
        GUI window;

        private BackgroundWorker updateInfoWorker;
        private BackgroundWorker kafkaWorker;

        //Informazioni per i backgroundWorker
        private string ownerRepository;
        private string accessToken;
        private string dataPath;
        private int tipoConnessione = 99;
        private string ipAddress = null;
        private string porta = null;

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            GUIContent content = new GUIContent("Connessione a Apache Kafka");
            menu.AddItem(content, false, MyCallback);
        }

        private void MyCallback()
        {
            new KafkaConnectionConfiguration();
        }

        public GUI()
        {
            this.dataPath = UnityEngine.Application.dataPath;
            this.isAlive = true;
            this.info = new Info();
            this.window = (GUI)EditorWindow.GetWindow(typeof(GUI));
            window.title = "GAP";
            this.username = PlayerPrefs.GetString("username");
            this.ownerRepository = PlayerPrefs.GetString("ownerRepository");
            this.accessToken = PlayerPrefs.GetString("accessToken");

            if(PlayerPrefs.HasKey("tipoConnessione"))
            {
                this.tipoConnessione = PlayerPrefs.GetInt("tipoConnessione");
            }
            this.ipAddress = PlayerPrefs.GetString("ipAddress");
            if(PlayerPrefs.HasKey("porta"))
            {
                this.porta = PlayerPrefs.GetString("porta");
            }
            ShowWindow();
        }

        private static void ShowWindow()
        {
            GetWindow<GUI>().Show();
        }

        public GUI(Info info)
        {
            this.dataPath = UnityEngine.Application.dataPath;
            this.ownerRepository = PlayerPrefs.GetString("ownerRepository");
            this.accessToken = PlayerPrefs.GetString("accessToken");
            this.isAlive = true;
            this.info = new Info();
            this.info=info;
            this.username = PlayerPrefs.GetString("username");
            this.window = (GUI)EditorWindow.GetWindow(typeof(GUI));
            window.title = "GAP";
            avatarUsers = new List<avatar>();
            using (var webClient = new WebClient())
            {
                imageUserBytes = webClient.DownloadData(info.infoPersonali.avatar_url);
                textureLogin = new Texture2D(10, 10);
                textureLogin.LoadImage(imageUserBytes);
                if(info.collaborators != null)
                {
                    for (int i = 0; i < info.collaborators.numberCollaborators(); ++i)
                    {
                        avatar Av = new avatar();
                        Av.username = info.collaborators.getUserByIndex(i).user.login;
                        Av.avatarUser = webClient.DownloadData(info.collaborators.getUserByIndex(i).user.avatar_url);
                        Av.textureAvatar = new Texture2D(8, 8);
                        Av.textureAvatar.LoadImage(Av.avatarUser);
                        avatarUsers.Add(Av);
                    }
                }
            }
            if (PlayerPrefs.HasKey("tipoConnessione"))
            {
                this.tipoConnessione = PlayerPrefs.GetInt("tipoConnessione");
            }
            this.ipAddress = PlayerPrefs.GetString("ipAddress");
            this.porta = PlayerPrefs.GetString("porta");
            window.Show(true);
        }

        public static void Init()
        { }

        void OnEnable()
        {
            this.updateInfoWorker = new BackgroundWorker();
            updateInfoWorker.WorkerReportsProgress = true;
            updateInfoWorker.WorkerSupportsCancellation = true;
            updateInfoWorker.DoWork += updateInfoWorker_DoWork;
            updateInfoWorker.ProgressChanged += updateInfoWorker_progressChanged;
            updateInfoWorker.RunWorkerCompleted += updateInfoWorker_workCompleted;
            updateInfoWorker.RunWorkerAsync();

            this.kafkaWorker = new BackgroundWorker();
            kafkaWorker.WorkerReportsProgress = true;
            kafkaWorker.WorkerSupportsCancellation = true;
            kafkaWorker.DoWork += kafkaWorker_DoWork;
            kafkaWorker.ProgressChanged += kafkaWorker_progressChanged;
            kafkaWorker.RunWorkerCompleted += kafkaWorker_workCompeted;
            kafkaWorker.RunWorkerAsync();

            this.thread = new Thread(() =>
              {
                  while (true)
                  {
                      if (!this.updateInfoWorker.IsBusy)
                      {
                          this.updateInfoWorker.CancelAsync();
                          Thread.Sleep(1000);
                          this.updateInfoWorker.RunWorkerAsync();
                      }
                      if (!this.kafkaWorker.IsBusy)
                      {
                          kafkaWorker.CancelAsync();
                          Thread.Sleep(1000);
                          kafkaWorker.RunWorkerAsync();
                      }
                  }
              });
            this.thread.Start();
        }

        void kafkaWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if(this.tipoConnessione!=99)
            {
                try
                {
                    this.kafkaOk = true;

                    //Invio status online al server Kafka
                    WebClient webClient = new WebClient();
                    try
                    {
                        char[] chars = { '-', 'T', ':', 'Z', ' ', '/' };
                        string[] data = DateTime.Now.ToString().Split(chars, StringSplitOptions.None);
                        webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");

                        string messaggio = username + "_:_" + data[0] + "-" + data[1] + "-" + data[2] + ":" + data[3] + "." + data[4] + "." + data[5];
                        if (this.tipoConnessione == 0)
                        {
                            webClient.DownloadString(new Uri("http://" + this.ipAddress + "/kafka/producer?message=" + messaggio));
                        }
                        else if (this.tipoConnessione == 1)
                        {
                            KafkaOptions options = new KafkaOptions();
                            options = new KafkaOptions(new Uri(this.ipAddress + ":" + this.porta));
                            BrokerRouter router = new BrokerRouter(options);
                            Producer client = new Producer(router);
                            router = new BrokerRouter(options);
                            client = new Producer(router);
                            client.SendMessageAsync("status", new[] { new Message(messaggio) }).Wait();
                        }
                    }
                    catch (Exception exc)
                    {
                        this.kafkaOk = false;
                    }

                    //Lettura status dal server online
                    List<String> lista = new List<string>();
                    try
                    {
                        string risultati = null;
                        Dictionary<String, String> messaggi = new Dictionary<string, string>();
                        //Caso di default
                        if (this.tipoConnessione == 0)
                        {
                            webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
                            risultati = webClient.DownloadString(new Uri("http://" + this.ipAddress + "/kafka/consumer"));
                            messaggi = JsonConvert.DeserializeObject<Dictionary<String, String>>(risultati);
                        }
                        else if (this.tipoConnessione == 1)
                        {
                            KafkaOptions options = new KafkaOptions();
                            options = new KafkaOptions(new Uri(this.ipAddress + ":" + this.porta));
                            BrokerRouter router = new BrokerRouter(options);
                            Consumer consumer = new Consumer(new ConsumerOptions("status", router));
                            foreach (var message in consumer.Consume())
                            {
                                string[] _message = message.ToString().Split('/');
                                messaggi.Add(_message[0], _message[1]);
                            }
                        }
                        List<String> listOnline = new List<String>();
                        if (messaggi != null)
                        {
                            if (messaggi.Count > 0)
                            {
                                foreach (var messaggio in messaggi)
                                {
                                    String[] message = null;
                                    String[] dtsplit = null;
                                    if (this.tipoConnessione == 1)
                                    {
                                        string[] separatingStrings = { "_:_" };
                                        message = messaggio.Value.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
                                        dtsplit = message[1].Split('-', ':', '.');
                                    }
                                    else
                                    {
                                        dtsplit = messaggio.Value.Split('-', ':', '.');
                                    }
                                    var date = DateTime.Now - new DateTime(int.Parse(dtsplit[2]), int.Parse(dtsplit[1]), int.Parse(dtsplit[0]), int.Parse(dtsplit[3]), int.Parse(dtsplit[4]), int.Parse(dtsplit[5]));
                                    if (double.Parse(date.TotalDays.ToString()) < 1)
                                    {
                                        if (double.Parse(date.TotalHours.ToString()) < 1)
                                        {
                                            if (double.Parse(date.TotalMinutes.ToString()) < 1)
                                            {
                                                if (double.Parse(date.TotalSeconds.ToString()) < 8)
                                                {
                                                    listOnline.Add(messaggio.Key);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        kafkaWorker.ReportProgress(0, listOnline);
                    }
                    catch (Exception exc)
                    {
                        this.kafkaOk = false;
                    }
                }
                catch (Exception exc)
                {
                    this.kafkaOk = false;
                }
            }
        }

        void kafkaWorker_progressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.info.collaborators.updateUsersStatusMassive((List<String>) e.UserState);
        }

        void kafkaWorker_workCompeted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!updateInfoWorker.IsBusy)
            {
                updateInfoWorker.CancelAsync();
            }
        }



        void updateInfoWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<basicInfo> eventi = new List<basicInfo>();
            List<basicInfo> listaUtenti = new List<basicInfo>();
            List<String> usersChecked = new List<string>();
            WebClient webClient = new WebClient();

            basicInfo info_1 = new basicInfo();
            try
            {
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.Headers.Add("Authorization", "Token " + this.accessToken);

                var s = this.dataPath.Split('/');

                var risultati = webClient.DownloadString("https://api.github.com/repos/" + this.ownerRepository + "/" + s[s.Length - 2] + "/events");

                string[] separatingStrings = { "[{\"", "\":\"", "\",\"", ",\"", "\",\"", "}}]", "}},{", "\":", "\":{\"", ",\"", "\"", "}", "]" };
                string[] details = risultati.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);

                info_1 = new basicInfo();
                bool check = false;
                for (int i = 0; i < details.Length - 1; i++)
                {
                    if (details[i].CompareTo("MemberEvent") == 0)
                    {
                        check = true;
                        info_1.lastActionDone = "Aggiunto nuovo collaboratore";
                    }
                    if (details[i].CompareTo("CreateEvent") == 0)
                    {
                        check = true;
                        info_1.lastActionDone = "Ha creato la repository";
                    }
                    if (details[i].CompareTo("actor") == 0)
                    {
                        info_1.username = details[i + 5];
                    }
                    if (details[i].CompareTo("commits") == 0 && !check)
                    {
                        info_1.lastActionDone = details[i + 10];
                    }
                    if (details[i].CompareTo("created_at") == 0)
                    {
                        info_1.whenDidLastActionDone = details[i + 1];

                        check = false;
                        if (!usersChecked.Contains(info_1.username))
                        {
                            eventi.Add(info_1);
                            usersChecked.Add(info_1.username);
                        }
                        info_1 = new basicInfo();
                    }
                }
                foreach (var evento in eventi)
                {
                    char[] chars = { '-', 'T', ':', 'Z' };
                    string[] dataEvento = evento.whenDidLastActionDone.Split(chars, StringSplitOptions.None);
                    string[] dataAttuale = DateTime.Now.ToString().Split(chars, StringSplitOptions.None);

                    double number = 0;
                    var date = DateTime.UtcNow - new DateTime(int.Parse(dataEvento[0]), int.Parse(dataEvento[1]), int.Parse(dataEvento[2]), int.Parse(dataEvento[3]), int.Parse(dataEvento[4]), int.Parse(dataEvento[5]));
                    string whenDidLastAction = null;
                    if (double.Parse(date.TotalDays.ToString()) >= 1)
                    {
                        number = Math.Round(double.Parse(date.TotalDays.ToString()), MidpointRounding.ToEven);
                        if (number > 1)
                            whenDidLastAction = number.ToString() + " days ago";
                        else
                            whenDidLastAction = number.ToString() + " day ago";
                    }
                    else
                    {
                        if (double.Parse(date.TotalHours.ToString()) >= 1)
                        {
                            number = Math.Round(double.Parse(date.TotalHours.ToString()), MidpointRounding.ToEven);
                            if (number == 1)
                                whenDidLastAction = number.ToString() + " hour ago";
                            else
                                whenDidLastAction = number.ToString() + " hours ago";
                        }
                        else
                        {
                            number = Math.Round(double.Parse(date.TotalMinutes.ToString()), MidpointRounding.ToEven);
                            if (number <= 1)
                            {
                                number = Math.Round(double.Parse(date.TotalSeconds.ToString()), MidpointRounding.ToEven);
                                if (number <= 10)
                                    whenDidLastAction = "Now";
                                else
                                    whenDidLastAction = "A little while ago";
                            }
                            else
                                whenDidLastAction = number.ToString() + " minutes ago";
                        }
                    }
                    string lastActionDone = null;
                    if (evento.lastActionDone != null)
                    {
                        if (evento.lastActionDone.CompareTo("") != 0)
                        {
                            lastActionDone = evento.lastActionDone;
                        }
                    }
                    if (evento.username.CompareTo(this.username) != 0)
                    {

                        basicInfo inform = new basicInfo();
                        inform.username = evento.username;
                        inform.lastActionDone = lastActionDone;
                        inform.whenDidLastActionDone = whenDidLastAction;
                        listaUtenti.Add(inform);
                    }
                }
            }
            catch (Exception exc)
            { }

            updateInfoWorker.ReportProgress(0, listaUtenti);
        }

        void updateInfoWorker_progressChanged(object sender, ProgressChangedEventArgs e)
        {
            List<basicInfo> lista = (List <basicInfo>)e.UserState;
            foreach(var item in lista)
            {
                this.info.collaborators.updateUser(item.username, item.lastActionDone, item.whenDidLastActionDone);
            }
        }

        void updateInfoWorker_workCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!updateInfoWorker.IsBusy)
            {
                updateInfoWorker.CancelAsync();
            }
        }

        void OnGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Box(textureLogin, GUILayout.Height(50), GUILayout.Width(50));
            GUILayout.BeginVertical();
            GUILayout.Label("");
            GUILayout.Label("Welcome "+ this.info.infoPersonali.login);
            GUILayout.EndVertical();
            if (this.kafkaOk)
                GUILayout.Label("Kafka Server Status: Connected");
            else
                GUILayout.Label("Kafka Server Status: Problems with the server");
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            foldoutOnline = EditorGUILayout.Foldout(foldoutOnline, "Online", foldoutOnlineControl);
            if (foldoutOnline)
            {
                if (foldoutOnlineControl)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    GUILayout.Label("");
                    if(this.info.collaborators!=null)
                    {
                        if(this.info.collaborators.listaCollaboratori.Count>0)
                        {
                            for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                            {
                                avatar a = new avatar();
                                if (this.avatarUsers != null)
                                {
                                    for (int y = 0; y < this.avatarUsers.Count; y++)
                                    {
                                        if (info.collaborators.getUserByIndex(i).status)
                                        {
                                            if (avatarUsers.ToArray()[y].username.CompareTo(info.collaborators.getUserByIndex(i).user.login) == 0)
                                            {
                                                a = avatarUsers.ToArray()[y];
                                                GUILayout.Box(a.textureAvatar, GUILayout.Height(38), GUILayout.Width(38));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Username");
                    if (this.info.collaborators != null)
                    {
                        if (this.info.collaborators.listaCollaboratori.Count > 0)
                        {
                            for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                            {
                                if (info.collaborators.getUserByIndex(i).status)
                                {
                                    GUILayout.Label(info.collaborators.getUserByIndex(i).user.login);
                                    GUILayout.Space(25);
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Last Action");
                    if (this.info.collaborators != null)
                    {
                        if (this.info.collaborators.listaCollaboratori.Count > 0)
                        {
                            for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                            {
                                if (info.collaborators.getUserByIndex(i).status)
                                {
                                    GUILayout.Label(info.collaborators.getUserByIndex(i).lastActionDone);
                                    GUILayout.Space(25);
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("When is done");

                    for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                    {
                        if (info.collaborators.getUserByIndex(i).status)
                        {
                            GUILayout.Label(info.collaborators.getUserByIndex(i).whenDidLastAction);
                            GUILayout.Space(25);
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                foldoutOnline = false;
            }

            //Per gli offline
            foldoutOffline = EditorGUILayout.Foldout(foldoutOffline, "Offline", foldoutOfflineControl);
            if (foldoutOffline)
            {
                if (foldoutOfflineControl)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    GUILayout.Label("");
                    if (this.info.collaborators != null)
                    {
                        if (this.info.collaborators.listaCollaboratori.Count > 0)
                        {
                            for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                            {
                                avatar a = new avatar();
                                if (this.avatarUsers != null)
                                {
                                    for (int y = 0; y < this.avatarUsers.Count; y++)
                                    {
                                        if (!info.collaborators.getUserByIndex(i).status)
                                        {
                                            if (avatarUsers.ToArray()[y].username.CompareTo(info.collaborators.getUserByIndex(i).user.login) == 0)
                                            {
                                                a = avatarUsers.ToArray()[y];
                                                GUILayout.Box(a.textureAvatar, GUILayout.Height(38), GUILayout.Width(38));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Username");
                    if (this.info.collaborators.listaCollaboratori != null)
                    {
                        if (this.info.collaborators.listaCollaboratori.Count > 0)
                        {
                            for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                            {
                                if (!info.collaborators.getUserByIndex(i).status)
                                {
                                    GUILayout.Label(info.collaborators.getUserByIndex(i).user.login);
                                    GUILayout.Space(25);
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Last Action");
                    if (this.info.collaborators != null)
                    {
                        if (this.info.collaborators.listaCollaboratori.Count > 0)
                        {
                            for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                            {
                                if (!info.collaborators.getUserByIndex(i).status)
                                {
                                    GUILayout.Label(info.collaborators.getUserByIndex(i).lastActionDone);
                                    GUILayout.Space(25);
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("When is done");

                    for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                    {
                        if (!info.collaborators.getUserByIndex(i).status)
                        {
                            GUILayout.Label(info.collaborators.getUserByIndex(i).whenDidLastAction);
                            GUILayout.Space(25);
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                foldoutOffline = false;
            }
        }

        public void updateUser(string username, string lastActionDone, string whenDidLastAction, bool status)
        {
            this.info.collaborators.updateUser(username, status);
            this.info.collaborators.updateUser(username, lastActionDone, whenDidLastAction);
        }

        public void updateUser(string username, string lastActionDone, string whenDidLastAction)
        {
            this.info.collaborators.updateUser(username, lastActionDone, whenDidLastAction);
        }

        void Update()
        { }

        void OnDestroy()
        {
            this.thread.Abort();
        }
    }
}
