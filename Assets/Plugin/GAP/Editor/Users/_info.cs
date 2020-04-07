using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Plugin.GAP.Editor.Users
{
    public class _info:ScriptableObject
    {
        public string username { get; set; }
        public string lastActionDone { get; set; }
        public string whenDidLastAction { get; set; }

        public _info() { }

        public _info(string username, string lastActionDone, string whenDidLastAction)
        {
            this.username = username;
            this.lastActionDone = lastActionDone;
            this.whenDidLastAction = whenDidLastAction;
        }
    }
}
