using System;
using UnityEditor;
using UnityEngine;
using Octokit;
using Assets.Plugins.GAP.Connection;
using System.Security.Permissions;
using Assets.Plugin.GAP.Editor;

namespace Assets.Plugins.GAP.Editor
{
    public class LoginGUI : EditorWindow
    {
        GitHubClient client;
        string username = null;
        string password = null;

        public LoginGUI()
        {
            var window = (LoginGUI)EditorWindow.GetWindow(typeof(LoginGUI));
            PlayerPrefs.DeleteKey("id");
            PlayerPrefs.DeleteKey("username");
            PlayerPrefs.DeleteKey("accessToken");
            PlayerPrefs.DeleteKey("ownerRepository");
            window.title = "GitHub Login";
            this.client = new GitHubClient(new ProductHeaderValue("GAP"));
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Inserisci i dati");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Username:");
            username = EditorGUILayout.TextArea(username);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Password:");
            password = EditorGUILayout.PasswordField(password);
            EditorGUILayout.EndHorizontal();


            bool button = GUILayout.Button("Login");
            if (button)
            {
                try
                {
                    GitHubConnection client = new GitHubConnection(username, password);
                }
                catch (Exception exc)
                {
                    Debug.Log(exc);
                }
                Debug.Log(PlayerPrefs.GetString("accessToken"));
                if (PlayerPrefs.GetString("accessToken") != null && PlayerPrefs.GetString("accessToken").CompareTo("") != 0)
                {
                    messageAlert alert=new messageAlert(this, "Login Effettuato");
                    this.Close();
                } else
                {
                    messageAlert alert = new messageAlert(null, "Credenziali non valide");
                }
            }
        }
    }
}
