using System;

namespace Assets.Plugins.GAP.Connection
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Net;
    using Newtonsoft.Json;
    using KafkaNet;
    using KafkaNet.Model;

    public class GAP_CONSUMER
    {
        public GAP_CONSUMER() { }

        //Returna gli username di chi è online
        public List<string> controlWhoIsOnline()
        {
            List<string> usernames = new List<string>();
            WebClient webClient = new WebClient();
            try
            {
                string risultati = null;
                Dictionary<String, String> messaggi = new Dictionary<string, string>();
                //Caso di default
                if (!PlayerPrefs.HasKey("tipoConnessione"))
                {
                    webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
                    risultati = webClient.DownloadString("http://kafkaserver.eu.ngrok.io:80/kafka/consumer");
                    messaggi = JsonConvert.DeserializeObject<Dictionary<String, String>>(risultati);
                } else if(PlayerPrefs.GetInt("tipoConnessione")==0)
                {
                    webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
                    risultati = webClient.DownloadString(PlayerPrefs.GetString("ipAddress"));
                    messaggi = JsonConvert.DeserializeObject<Dictionary<String, String>>(risultati);
                } else if(PlayerPrefs.GetInt("tipoConnessione") == 1)
                {
                    var options = new KafkaOptions(new Uri(PlayerPrefs.GetString("ipAddress")+PlayerPrefs.GetString("porta")));
                    var router = new BrokerRouter(options);
                    var consumer = new Consumer(new ConsumerOptions("status", router));
                    foreach (var message in consumer.Consume())
                    {
                        string[] _message=message.ToString().Split('/');
                        messaggi.Add(_message[0], _message[1]);
                    }
                }

                if (messaggi!=null)
                {
                    if (messaggi.Count > 0)
                    {
                        foreach (var messaggio in messaggi)
                        {
                            String[] dtsplit = messaggio.Value.Split('-', ':', '.');
                            var date = DateTime.Now - new DateTime(int.Parse(dtsplit[2]), int.Parse(dtsplit[1]), int.Parse(dtsplit[0]), int.Parse(dtsplit[3]), int.Parse(dtsplit[4]), int.Parse(dtsplit[5]));
                            List<String> listOnline = new List<String>();
                            if (double.Parse(date.TotalDays.ToString()) < 1)
                            {
                                if (double.Parse(date.TotalHours.ToString()) < 1)
                                {
                                    if (double.Parse(date.TotalMinutes.ToString()) < 1)
                                    {
                                        if (double.Parse(date.TotalSeconds.ToString()) < 8)
                                        {
                                            listOnline.Add(messaggio.Key);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return usernames;
            }
            catch (FormatException exc)
            {
                return null;
            }
            catch (Exception exc)
            {
                Debug.Log(exc);
                return null;
            }
        }
    }
}

