using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;

namespace Assets.Plugins.GAP.Analysis
{
    public class AnalysisMessage
    {
        Thread thread;

        public AnalysisMessage()
        {
            thread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
            });
            thread.Start();
        }

        public string[] userStatus(string message)
        {
            string[] dettagli = null;
            if (message.Contains(":") || message.Contains("/") || message.Contains(" "))
                dettagli = message.ToString().Split(' ', ':', '/');
            return dettagli;
        }

        public string[] userInfo(string message)
        {
            string[] dettagli = message.ToString().Split('/');
            return dettagli;
        }

        public string[] actualDateToString()
        {
            string[] date = DateTime.Now.ToString().Split(' ', ':', '/');
            return date;
        }

        public string[] splitCollaborators(string message)
        {
            string[] separatingStrings = { "[{\"", "\":\"", "\",\"", ",\"", "\",\"", "}}]", "}},{", "\":", "\":{\"", ",\"", "\""};
            string[] details = message.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            return details;
        }

        public DateTime actualDateToDate()
        {
            return DateTime.Now;
        }

        public DateTime convertStringToDate(string year, string month, string day, string hour, string min, string second)
        {
            return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(min), int.Parse(second));
        }

        public string[] dateToArrayString(DateTime date)
        {
            return date.ToString().Split(':');
        }

        public int searchClientIdByUsername(string username, string message)
        {
            int clientId = 0;

            string[] details = message.Split('{', '[', '}', '}', ':', '\"');
            Debug.Log(details);


            return clientId;
        }
    }
}
