using Assets.Plugin.GAP.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Plugins.GAP.Editor
{
    public class KafkaConnectionConfiguration : EditorWindow
    {
        private string ipAddress="";
        private string port="";
        private int index;

        public KafkaConnectionConfiguration()
        {
            KafkaConnectionConfiguration window=GetWindow<KafkaConnectionConfiguration>();
            window.title = "Configurazione Connessione";
            window.Show();
        }

        public static void Init()
        { }

        void OnDestroy()
        { }

        void OnEnable()
        { }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Indirizzo IP/Hostname:");
            ipAddress = EditorGUILayout.TextArea(ipAddress);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Porta:");
            port = EditorGUILayout.TextArea(port);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Scegli il tipo di connessione con Apache Kafka:");
            index = EditorGUILayout.Popup(index, new string[] { "REST API", "Connessione diretta" });
            if (GUILayout.Button("Connetti"))
            {
                if(ipAddress.Equals(""))
                {
                    new messageAlert(null, "IpAddress/Hostname mancante");
                } else if(port.Equals("") && index!=1)
                {
                    new messageAlert(null, "Porta mancante");
                } else
                {
                    PlayerPrefs.SetInt("tipoConnessione", index);
                    PlayerPrefs.SetString("ipAddress", ipAddress);
                    PlayerPrefs.SetString("porta", port);
                    this.Close();
                }
            }
        }
    }
}
