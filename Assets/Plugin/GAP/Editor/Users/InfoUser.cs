using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Plugins.GAP.Editor.Users
{
    public class InfoUser
    {
        public Collaborator user { get; set;}
        public string lastActionDone { get; set; }
        public string whenDidLastAction { get; set; }
        public bool status { get; set; }

        public InfoUser(Collaborator user, string lastActionDone, string whenDidLastAction, bool status)
        {
            this.user = user;
            this.lastActionDone = lastActionDone;
            this.whenDidLastAction = whenDidLastAction;
            this.status = status;
        }

        public void Awake()
        { }

        public void OnEnable()
        { }

        public void OnDisable()
        { }

        public void OnDestroy()
        { }
    }
}
