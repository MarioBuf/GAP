using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Plugins.GAP.Editor.Users
{
    public class ListaCollaboratori : ScriptableObject
    {
        public List<InfoUser> listaCollaboratori {get; set;}

        public ListaCollaboratori() {
            this.listaCollaboratori = new List<InfoUser>();
        }

        public void addUser(Collaborator collaboratore, string lastActionDone, string whenDidLastAction, bool status)
        {
            int i = 0;
            foreach (var user in this.listaCollaboratori)
            {
                if (user.user.login.CompareTo(collaboratore.login) == 0)
                {
                    listaCollaboratori.RemoveAt(i);
                }
            }
            listaCollaboratori.Add(new InfoUser(collaboratore, lastActionDone, whenDidLastAction, status));
        }

        public InfoUser getUser(string username)
        {
            for (int i = 0; i < this.numberCollaborators(); ++i)
            {
                if (this.getUserByIndex(i).user.login.Equals(username))
                    return listaCollaboratori.ToArray()[i];
            }
            return null;
        }

        public InfoUser getUserByIndex(int index)
        {
            return this.listaCollaboratori.ToArray()[index];
        }

        public void updateUsersStatusMassive(List<string> usernameList)
        {
            foreach (var collaboratore in this.listaCollaboratori)
            {
                int index = this.listaCollaboratori.IndexOf(collaboratore);
                if (usernameList.Contains(collaboratore.user.login))
                {
                    this.listaCollaboratori[index].status = true;
                }
                else
                {
                    this.listaCollaboratori[index].status = false;
                }
            }

        }

        public bool ifExist(string username)
        {
            for (int i = 0; i < this.numberCollaborators(); ++i)
            {
                if (this.getUserByIndex(i).user.login.Equals(username))
                    return true;
            }
            return false;
        }


        public void updateUser(string username, bool status)
        {
            for (int i = 0; i < this.numberCollaborators(); ++i)
            {
                if (this.getUserByIndex(i).user.login.Equals(username))
                {
                    this.listaCollaboratori.ToArray()[i].status = status;
                }
            }
        }

        public void updateUser(string username, string lastActionDone, string whenDidLastAction)
        {
            for (int i = 0; i < this.numberCollaborators(); ++i)
            {
                if (this.getUserByIndex(i).user.login.CompareTo(username)==0)
                {
                    if (lastActionDone != null)
                        this.listaCollaboratori.ToArray()[i].lastActionDone = lastActionDone;
                    if (whenDidLastAction != null)
                        this.listaCollaboratori.ToArray()[i].whenDidLastAction = whenDidLastAction;
                }
            }
        }

        private int getIndex(string username)
        {
            for (int i = 0; i < this.numberCollaborators(); ++i)
            {
                if (this.listaCollaboratori.ToArray()[i].user.login.Equals(username))
                    return i;
            }
            return -1;
        }

        public bool removeUser(string username)
        {
            for (int i = 0; i < this.numberCollaborators(); ++i)
            {
                if (this.listaCollaboratori.ToArray()[i].user.login.Equals(username))
                {
                    this.listaCollaboratori.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public int numberCollaborators()
        {
            return this.listaCollaboratori.Count;
        }
    }
}
