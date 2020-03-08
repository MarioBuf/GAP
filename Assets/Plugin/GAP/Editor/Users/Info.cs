using Assets.Plugins.GAP.Connection;
using Assets.Plugins.GAP.Editor.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Plugins.GAP.Connection.GitHubConnection;

namespace Assets.Plugins.GAP.Editor.Users
{
    public class Info : ScriptableObject
    {
        public Collaborator infoPersonali { get; set; }
        public ListaCollaboratori collaborators { get; set; }
        public GitHubConnection account { get; set; }

        public Info()
        {
            this.infoPersonali = new Collaborator();
            this.account = new GitHubConnection();
            this.collaborators = new ListaCollaboratori();
        }

        public bool setInfo()
        {
            bool control = false;
            if (this.account.checkValidateInfo())
            {
                try
                {
                    var temp1 = this.infoPersonali = account.getProfile();
                    var temp2 = this.collaborators = this.account.getInfoCollaboratorsRepository();
                    this.updateInfoCollaborators();
                    if (temp1 != null && temp2 != null)
                        control = true;
                    else
                        control = false;
                } catch(Exception exc)
                {
                    control = false;
                }
            }
            return control;
        }

        public void updateInfoCollaborators()
        {
            List<_info> listaInfo = new List<_info>();
            listaInfo = this.account.getInfoAction();
            for (int i=0; i<collaborators.numberCollaborators(); ++i)
            {
                for (int y=0; y<listaInfo.Count; ++y)
                {
                    if (collaborators.getUserByIndex(i).user.login.CompareTo(listaInfo.ToArray()[y].username)==0)
                    {
                        //InfoUser infoUser = new InfoUser(collaborators.getUserByIndex(i).user, elemento.lastActionDone, elemento.whenDidLastAction, item.status);
                        collaborators.updateUser(listaInfo.ToArray()[y].username, listaInfo.ToArray()[y].lastActionDone, listaInfo.ToArray()[y].whenDidLastAction);
                    }
                }
            }
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
