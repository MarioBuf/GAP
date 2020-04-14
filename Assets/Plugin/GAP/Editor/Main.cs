#pragma warning(push, 0)
using Unity;
using UnityEngine;
using UnityEditor;
#pragma warning(pop)

namespace Assets.Plugins.GAP.Editor
{
    using Assets.Plugin.GAP.Editor;
    using Assets.Plugins.GAP.Connection;
    using Assets.Plugins.GAP.Editor.Users;
    using Connection;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Security.Permissions;
    using System.Threading;
    using UnityEngine;

    public class Main
    {
        GUI window;
        Info info;

        [MenuItem("Window/GAP")]
        public static void Init()
        {
            new Main().startGAP();
        }

        void OnEnable()
        { }

        private void startGAP()
        {
            this.info = new Info();

            bool control = true;
            if (this.info.account.checkValidateInfo())
            {
                this.info.setInfo();
            }
            else
            {
                control = false;
            }

            if (control)
            {
                if (PlayerPrefs.HasKey("tipoConnessione"))
                {
                    window = new GUI(this.info);
                } else
                {
                    new messageAlert(null, "E' necessario configurare la connessione ad Apache Kafka");
                    new KafkaConnectionConfiguration();
                }
            } else
            {
                PlayerPrefs.DeleteKey("id");
                PlayerPrefs.DeleteKey("username");
                PlayerPrefs.DeleteKey("accessToken");
                PlayerPrefs.DeleteKey("ownerRepository");
                new Login();
            }
        }

        void OnDestroy()
        {

        }
    }
}