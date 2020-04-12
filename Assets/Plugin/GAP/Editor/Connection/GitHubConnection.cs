using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using UnityEngine;

namespace Assets.Plugins.GAP.Connection
{
    using Editor;
    using System.Security.Cryptography;
    using System.Security.Permissions;
    using Newtonsoft.Json;
    using Assets.Plugins.GAP.Editor.Users;
    using Assets.Plugins.GAP.Editor.Connection;
    using System.Collections.Specialized;
    using Assets.Plugin.GAP.Editor.Users;

    public class GitHubConnection
    {
        
        public GitHubConnection()
        {}

        public GitHubConnection(string username, string password)
        {
            Credentials credenziali = new Credentials(username, password);
            GitHubClient client = new GitHubClient(new Octokit.ProductHeaderValue("GAP"));
            client.Credentials = credenziali;
            PlayerPrefs.SetString("username", username);
            deleteToken(username, password);
            this.setAccessToken(username, password);
            this.setOwnerRepository();
        }

        public void setOwnerRepository()
        {
            WebClient webClient = new WebClient();
            try
            {
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.Headers.Add("Authorization", "Token " + PlayerPrefs.GetString("accessToken"));
                var risultati = webClient.DownloadString("https://api.github.com/user/repos");
                string[] separatingStrings = { "[{\"", "\":\"", "\",\"", ",\"", "\",\"", "}}]", "}},{", "\":", "\":{\"", ",\"", "\"" };
                String[] lista = risultati.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
                var dp = UnityEngine.Application.dataPath;
                var s = dp.Split('/');
                String[] ownerT = null;
                for (int i=0; i<lista.Length; ++i)
                {
                    if(lista[i].CompareTo(s[s.Length - 2].ToString())==0)
                    {
                        ownerT = lista[i + 2].Split('/');
                        PlayerPrefs.SetString("ownerRepository", ownerT[0]);
                        Debug.Log("OWNER_REPOSITORY: " + ownerT[0]);
                    }
                }
            } catch (Exception exc)
            {
                Debug.Log("ERRORE");
            }
        }

        public void deleteToken(String username, String password)
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
            var tokenCred = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            client.Headers.Add("Authorization", "Basic " + tokenCred);
            var response = client.DownloadString("https://api.github.com/authorizations");
            List<Authorizazion> authorizations = JsonConvert.DeserializeObject<List<Authorizazion>>(response);
            if(authorizations.Count>0)
            {
                foreach (var item in authorizations)
                {
                    if (item.app._name.CompareTo("GAP") == 0)
                    {
                        GitHubClient n = new GitHubClient(new ProductHeaderValue("GAP"));
                        n.Credentials = new Credentials(username, password);
                        var task=n.Authorization.Delete(item.id);
                    }
                }
            }
        }

        public bool checkValidateInfo()
        {
            bool control = false;
            if(PlayerPrefs.HasKey("username") && PlayerPrefs.HasKey("accessToken"))
            {
                WebClient webClient = new WebClient();
                try
                {
                    webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
                    webClient.Headers.Add("Authorization", "Token " + PlayerPrefs.GetString("accessToken"));
                    var risultati = webClient.DownloadString("https://api.github.com/user");
                    if (risultati.Contains("Bad credentials"))
                    {
                        PlayerPrefs.SetString("accessToken", null);
                        control = false;
                    }
                    else
                    {
                        control = true;
                    }
                }
                catch (Exception exc)
                {
                    control = false;
                }
            } else
            {
                control = false;
            }
            return control;
        }

        public void setAccessToken(string username, string password)
        {
            try
            {
                GitHubClient webClient = new GitHubClient(new Octokit.ProductHeaderValue("GAP"));
                if(!checkValidateInfo())
                {
                    this.setOwnerRepository();
                    this.deleteToken(username, password);
                    List<string> scope = new List<string>();
                    scope.Add("repo");
                    scope.Add("write:packages");
                    scope.Add("read:packages");
                    scope.Add("delete:packages");
                    scope.Add("admin:org");
                    scope.Add("admin:public_key");
                    scope.Add("admin:repo_hook");
                    scope.Add("admin:org_hook");
                    scope.Add("gist");
                    scope.Add("notifications");
                    scope.Add("user");
                    scope.Add("delete_repo");
                    scope.Add("write:discussion");
                    scope.Add("admin:enterprise");
                    scope.Add("workflow");
                    scope.Add("admin:gpg_key");
                    webClient.Credentials = new Credentials(username, password);
                    Task<ApplicationAuthorization> accessToken = webClient.Authorization.Create(new NewAuthorization("GAP", scope));
                    PlayerPrefs.SetString("accessToken", accessToken.Result.Token);
                    PlayerPrefs.SetString("username", username);
                }
            }
            catch (Exception exc)
            {
                Debug.Log("setAccessToken"+exc);
            }
        }

        public ListaCollaboratori getInfoCollaboratorsRepository()
        {
            WebClient webClient = new WebClient();
            try
            {
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.Headers.Add("Authorization", "Token " + PlayerPrefs.GetString("accessToken"));
                var dp = UnityEngine.Application.dataPath;
                var s = dp.Split('/');
                var risultati = webClient.DownloadString("https://api.github.com/repos/" + PlayerPrefs.GetString("ownerRepository") + "/" + s[s.Length - 2].ToString() + "/collaborators");
                List<Collaborator> lista = JsonConvert.DeserializeObject<List<Collaborator>>(risultati);
                ListaCollaboratori listaCollaboratori=new ListaCollaboratori();
                foreach (var collaboratore in lista)
                {
                    if(collaboratore.login.CompareTo(PlayerPrefs.GetString("username"))!=0)
                        listaCollaboratori.addUser(collaboratore, "Nessuna Azione", "Mai", false);
                }
                this.getInfoAction();
                return listaCollaboratori;
            }
            catch (HttpRequestException e)
            {
                Debug.Log("\nException Caught!");
                Debug.Log("Message :{0} " + e.Message.ToString());
                return null;
            }
            catch (Exception exc)
            {
                Debug.Log("\nException Caught!");
                Debug.Log("Message :{0} " + exc.Message.ToString());
                return null;
            }
        }

        public Lista_info getInfoAction()
        {
            List<Event> eventi;
            Lista_info listaUtenti = new Lista_info();
            WebClient webClient = new WebClient();
            try
            {
                this.setOwnerRepository();
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.Headers.Add("Authorization", "Token " + PlayerPrefs.GetString("accessToken"));
                var dp = UnityEngine.Application.dataPath;
                var s = dp.Split('/');

                var risultati = webClient.DownloadString("https://api.github.com/repos/" + PlayerPrefs.GetString("ownerRepository") + "/" + s[s.Length - 2] + "/events");
                eventi = JsonConvert.DeserializeObject<List<Event>>(risultati);
                bool check = false;
                foreach (var evento in eventi)
                {
                    check = false;
                    for (int i=0; i<listaUtenti.lista_info.Count; ++i)
                    {
                        if (listaUtenti.lista_info[i].username.CompareTo(evento.actor.login) == 0)
                        {
                            check = true;
                        }
                    }
                    if (!check)
                    {
                        char[] chars = { '-', 'T', ':', 'Z'};
                        string[] dataEvento = evento.created_at.Split(chars, StringSplitOptions.None);
                        //Debug.Log(dataEvento[0] + "\n" + dataEvento[1] + "\n" + dataEvento[2] + "\n" + dataEvento[3] + "\n" + dataEvento[4] + "\n" + dataEvento[5]);
                        string[] dataAttuale = DateTime.Now.ToString().Split(chars, StringSplitOptions.None);
                        double number=0; //for check days, hour or minutes from last action
                        var date = DateTime.UtcNow - new DateTime(int.Parse(dataEvento[0]), int.Parse(dataEvento[1]), int.Parse(dataEvento[2]), int.Parse(dataEvento[3]), int.Parse(dataEvento[4]), int.Parse(dataEvento[5]));
                        string whenDidLastAction = null;
                        //Debug.Log("Giorni: " + date.TotalDays + "\nOre: " + date.TotalHours + "\nMinuti: " + date.TotalMinutes);
                        if (double.Parse(date.TotalDays.ToString()) >= 1)
                        {
                            //number = double.Parse(date.TotalDays.ToString());
                            number = Math.Round(double.Parse(date.TotalDays.ToString()), MidpointRounding.ToEven);
                            if(number>1)
                                whenDidLastAction = number.ToString() + " giorni fa";
                            else
                                whenDidLastAction = number.ToString() + " giorno fa";
                        } else
                        {
                            if(double.Parse(date.TotalHours.ToString()) >= 1)
                            {
                                number = Math.Round(double.Parse(date.TotalHours.ToString()), MidpointRounding.ToEven);
                                if(number==1)
                                    whenDidLastAction = number.ToString() + " ora fa";
                                else
                                    whenDidLastAction = number.ToString() + " ore fa";
                            } else
                            {
                                number = Math.Round(double.Parse(date.TotalMinutes.ToString()), MidpointRounding.ToEven);
                                if(number==1)
                                    whenDidLastAction = number.ToString() + " minuto fa";
                                else
                                    whenDidLastAction = number.ToString() + " minuti fa";
                            }
                        }
                        string lastActionDone = null;
                        if (evento.payload.commits!=null)
                        {
                            if (evento.payload.commits[0].message != null && evento.payload.commits[0].message.CompareTo("") != 0)
                            {
                                lastActionDone = evento.payload.commits[0].message;
                            }
                        } else
                        {
                            lastActionDone = evento.type;
                        }
                        _info info=new _info();
                        info.username = evento.actor.login;
                        info.whenDidLastAction = whenDidLastAction;
                        info.lastActionDone = lastActionDone;
                        listaUtenti.lista_info.Add(info);
                    }
                }
                return listaUtenti;
            } catch(Exception exc)
            {
                Debug.Log("\nException Caught!");
                Debug.Log("Message :{0} " + exc.Message.ToString());
                return null;
            }
        }

        public Collaborator getProfile()
        {
            WebClient webClient = new WebClient();
            try
            {
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.Headers.Add("Authorization", "Token " + PlayerPrefs.GetString("accessToken"));
                var result = webClient.DownloadString("https://api.github.com/users/" + PlayerPrefs.GetString("username"));
                Collaborator info = new Collaborator(); 
                info=JsonConvert.DeserializeObject<Collaborator>(result);
                PlayerPrefs.SetInt("id", info.id);
                PlayerPrefs.SetString("username", info.login);
                return info;
            }
            catch (HttpRequestException e)
            {
                Debug.Log("\nException Caught!");
                Debug.Log("Message :{0} " + e.Message.ToString());
                return null;
            }
            catch (Exception exc)
            {
                Debug.Log("\nException Caught!");
                Debug.Log("Message :{0} " + exc.Message.ToString());
                return null;
            }
        }
    }
}