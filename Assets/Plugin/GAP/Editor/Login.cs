using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using System.Security.Permissions;
using System.Threading;
using UnityEngine;
using Assets.Plugins.GAP.Connection;

namespace Assets.Plugins.GAP.Editor
{
    class Login
    {
        public Login()
        {
            this.startLogin();
        }

        [MenuItem("Window/GitHub Login")]
        public static void Init()
        {
            new Login();
        }

        void startLogin()
        {
            LoginGUI window = new LoginGUI();
        }
    }
}
