using Assets.Plugin.GAP.Editor.Users;
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
                    var infoActions = this.account.getInfoAction();
                    foreach (var infoA in infoActions.lista_info)
                    {
                        foreach(var collaboratore in this.collaborators.listaCollaboratori)
                        {
                            if(collaboratore.user.login.CompareTo(infoA.username) == 0)
                            {
                                this.collaborators.updateUser(infoA.username, infoA.lastActionDone, infoA.whenDidLastAction);
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
            Lista_info listaInfo = new Lista_info();
            listaInfo = this.account.getInfoAction();
            for (int i=0; i<collaborators.numberCollaborators(); ++i)
            {
                for (int y=0; y<listaInfo.lista_info.Count; ++y)
                {
                    if (collaborators.getUserByIndex(i).user.login.CompareTo(listaInfo.lista_info[y].username)==0)
                    {
                        //InfoUser infoUser = new InfoUser(collaborators.getUserByIndex(i).user, elemento.lastActionDone, elemento.whenDidLastAction, item.status);
                        collaborators.updateUser(listaInfo.lista_info[y].username, listaInfo.lista_info[y].lastActionDone, listaInfo.lista_info[y].whenDidLastAction);
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
