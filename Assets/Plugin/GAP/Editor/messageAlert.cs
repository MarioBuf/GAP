using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Plugin.GAP.Editor
{
    public class messageAlert : EditorWindow
    {
        private string message;
        private EditorWindow gui;
        public messageAlert(EditorWindow gui, string message, string title)
        {
            this.gui = gui;
            this.message = message;
            messageAlert window = GetWindow<messageAlert>();
            if (title == null)
                window.title = "Errore";
            else
                window.title = title;
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
            EditorGUILayout.LabelField(this.message);
            if (GUILayout.Button("OK"))
            {
                if (this.gui != null)
                {
                    this.gui.Close();
                }
                this.Close();
            }
        }
    }
}
