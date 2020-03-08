using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Assets.Plugins.GAP.Editor.Users;
using Assets.Plugins.GAP.Connection;
using System.Net;
using System.Threading;

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
        private String username;

        //Altro
        GAP_CONSUMER consumer;
        GAP_PRODUCER producer;
        List<String> lista = new List<String>();
        bool isAlive = false;


        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            GUIContent content = new GUIContent("Connessione a Apache Kafka");
            menu.AddItem(content, false, MyCallback);
        }

        private void MyCallback()
        {
            new KafkaConnectionConfiguration();
            Debug.Log("My Callback was called.");
        }

        public GUI()
        {
            this.isAlive = true;
            this.info = new Info(); //(ListaUtenti)ScriptableObject.CreateInstance(typeof(ListaUtenti));
            GUI window = (GUI)EditorWindow.GetWindow(typeof(GUI));
            window.title = "GAP";
            this.username = PlayerPrefs.GetString("username");

            ShowWindow();
        }

        private static void ShowWindow()
        {
            GetWindow<GUI>().Show();
        }


        public GUI(Info info)
        {
            this.isAlive = true;
            this.info = new Info();
            this.info=info;
            this.username = PlayerPrefs.GetString("username");
            GUI window = (GUI)EditorWindow.GetWindow(typeof(GUI));
            window.title = "GAP";
            avatarUsers = new List<avatar>();
            using (var webClient = new WebClient())
            {
                imageUserBytes = webClient.DownloadData(info.infoPersonali.avatar_url);
                textureLogin = new Texture2D(10, 10);
                textureLogin.LoadImage(imageUserBytes);
                for (int i=0; i<info.collaborators.numberCollaborators(); ++i)
                {
                    avatar Av = new avatar();
                    Av.username = info.collaborators.getUserByIndex(i).user.login;
                    Av.avatarUser = webClient.DownloadData(info.collaborators.getUserByIndex(i).user.avatar_url);
                    Av.textureAvatar = new Texture2D(8, 8);
                    Av.textureAvatar.LoadImage(Av.avatarUser);
                    avatarUsers.Add(Av);
                }
            }
            window.Show();
        }

        public static void Init()
        { }

        void OnEnable()
        {
            this.consumer = new GAP_CONSUMER();
            this.producer = new GAP_PRODUCER();
            this.username = PlayerPrefs.GetString("username");
            Thread thread = new Thread(() =>
            {
                while (this.isAlive)
                {
                    Thread.CurrentThread.IsBackground = true;
                    producer.sendMessage(this.username);
                    Thread.Sleep(1000);
                    this.lista = consumer.controlWhoIsOnline();
                    if (lista != null)
                    {
                        if (lista.Count != 0)
                        {
                            this.info.collaborators.updateUsersStatusMassive(lista);
                        }
                    }
                    Thread.Sleep(5000);
                }
            });
            thread.Start();
        }

        void OnGUI()
        {
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Box(textureLogin, GUILayout.Height(50), GUILayout.Width(50));
            GUILayout.BeginVertical();
            GUILayout.Label("");
            GUILayout.Label("Benvenuto "+PlayerPrefs.GetString("username"));
            GUILayout.EndVertical();
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
                    for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                    {
                        avatar a = new avatar();
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
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Username");
                    for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                    {
                        if (info.collaborators.getUserByIndex(i).status)
                        {
                            GUILayout.Label(info.collaborators.getUserByIndex(i).user.login);
                            GUILayout.Space(25);
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Last Action");
                    for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                    {
                        if (info.collaborators.getUserByIndex(i).status)
                        {
                            GUILayout.Label(info.collaborators.getUserByIndex(i).lastActionDone);
                            GUILayout.Space(25);
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
                    for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                    {
                        avatar a = new avatar();
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
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Username");
                    for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                    {
                        if (!info.collaborators.getUserByIndex(i).status)
                        {
                            GUILayout.Label(info.collaborators.getUserByIndex(i).user.login);
                            GUILayout.Space(25);
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Last Action");
                    for (int i = 0; i < this.info.collaborators.listaCollaboratori.Count; ++i)
                    {
                        if (!info.collaborators.getUserByIndex(i).status)
                        {
                            GUILayout.Label(info.collaborators.getUserByIndex(i).lastActionDone);
                            GUILayout.Space(25);
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

        public InfoUser getUser(string username)
        {
            return this.info.collaborators.getUser(username);
        }

        public Info getList()
        {
            return this.info;
        }

     

        void OnDestroy()
        {
            this.isAlive = false;
        }
    }
}
