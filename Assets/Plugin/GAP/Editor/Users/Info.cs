using Assets.Plugins.GAP.Connection;
using System;
using System.Linq;
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
                    var infoActions = this.account.getInfoAction();
                    foreach (var infoA in infoActions.listaInfo)
                    {
                        foreach(var collaboratore in this.collaborators.listaCollaboratori)
                        {
                            if(collaboratore.user.login.CompareTo(infoA.username) == 0)
                            {
                                this.collaborators.updateUser(infoA.username, infoA.lastActionDone, infoA.whenDidLastActionDone);
                            }
                        }
                    }
                } catch(Exception exc)
                {
                    control = false;
                }
            }
            return control;
        }

        public void updateInfoCollaborators()
        {
            var listaInfo = this.account.getInfoAction();
            for (int i=0; i<collaborators.numberCollaborators(); ++i)
            {
                for (int y=0; y<listaInfo.listaInfo.Count; ++y)
                {
                    if (collaborators.getUserByIndex(i).user.login.CompareTo(listaInfo.listaInfo[y].username)==0)
                    {
                        collaborators.updateUser(listaInfo.listaInfo[y].username, listaInfo.listaInfo[y].lastActionDone, listaInfo.listaInfo[y].whenDidLastActionDone);
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
