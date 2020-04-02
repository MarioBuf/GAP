using System;
using KafkaNet;
using KafkaNet.Model;
using System.Net;
using UnityEngine;
using KafkaNet.Protocol;

namespace Assets.Plugins.GAP.Connection
{
    public class GAP_PRODUCER
    {
        KafkaOptions options;
        BrokerRouter router;
        Producer client;
        String Username;

        public GAP_PRODUCER()
        {
            this.startProducer();
        }

        public void startProducer()
        {
            if (PlayerPrefs.GetInt("tipoConnessione") == 0)
            {
                this.options = new KafkaOptions(new Uri(PlayerPrefs.GetString("ipAddress")));
            }
            else if (PlayerPrefs.GetInt("tipoConnessione") == 1)
            {
                this.options = new KafkaOptions(new Uri(PlayerPrefs.GetString("ipAddress") + PlayerPrefs.GetString("porta")));
            }
            this.router = new BrokerRouter(options);
            this.client = new Producer(router);
            var topics=this.router.GetTopicMetadata();
        }

        public void sendMessage(String username)//(string message, string topic)
        {
            WebClient webClient = new WebClient();
            try
            {
                char[] chars = { '-', 'T', ':', 'Z', ' ', '/' };
                string[] data = DateTime.Now.ToString().Split(chars, StringSplitOptions.None);
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
                string messaggio = username + "_:_" + data[0] + "-" + data[1] + "-" + data[2] + ":" + data[3] + "." + data[4] + "." + data[5];
                byte[] risultati = null;
                if (!PlayerPrefs.HasKey("tipoConnessione"))
                {
                    risultati = webClient.DownloadData("http://kafkaserver.eu.ngrok.io:80/kafka/producer?message=" + messaggio);
                } else if(PlayerPrefs.GetInt("tipoConnessione") == 0)
                {
                    risultati = webClient.DownloadData(PlayerPrefs.GetString("ipAddress"));
                } else
                {
                    var options = new KafkaOptions(new Uri(PlayerPrefs.GetString("ipAddress")+ PlayerPrefs.GetString("porta")));
                    var router = new BrokerRouter(options);
                    var client = new Producer(router);
                    client.SendMessageAsync("status", new[] { new Message(messaggio) }).Wait();
                }
            }
            catch (FormatException exc)
            {
                Debug.Log("Stringa Incorretta");
            }
            catch (Exception exc)
            {
                Debug.Log(exc);
            }
        }
    }
}
