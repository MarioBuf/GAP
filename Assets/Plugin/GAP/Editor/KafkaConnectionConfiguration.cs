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
            window.title = "Connection Setting";
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
            EditorGUILayout.LabelField("IP Address/Hostname:");
            ipAddress = EditorGUILayout.TextArea(ipAddress);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Port:");
            port = EditorGUILayout.TextArea(port);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Choose type of connection to Apache Kafka:");
            index = EditorGUILayout.Popup(index, new string[] { "Kafka-Connect", "Direct Connection" });
            if (GUILayout.Button("Connect"))
            {
                if (ipAddress.Equals(""))
                {
                    new messageAlert(null, "Miss IpAddress/Hostname", null);
                } else if(port.Equals("") && index==1)
                {
                    new messageAlert(null, "Miss Port", null);
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
