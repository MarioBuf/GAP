using Unity;
using UnityEngine;
using UnityEditor;

namespace Assets.Plugins.GAP.Editor
{
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
        GAP_CONSUMER consumer;
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
                window = new GUI(this.info);
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