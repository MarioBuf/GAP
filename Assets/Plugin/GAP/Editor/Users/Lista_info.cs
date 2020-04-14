using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Plugin.GAP.Editor.Users
{
    public class Lista_info : ScriptableObject
    {
        public List<basicInfo> listaInfo { get; set; }

        public Lista_info()
        {
            this.listaInfo = new List<basicInfo>();
        }
    }
}
