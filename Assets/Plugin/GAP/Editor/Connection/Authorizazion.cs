using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Plugins.GAP.Editor.Connection
{
    public class App: ScriptableObject
    {
        [JsonProperty("name")]
        public string _name { get; set; }
        public string url { get; set; }
        public string client_id { get; set; }
    }
    public class Authorizazion: ScriptableObject
    {
        public int id { get; set; }
        public string url { get; set; }
        [JsonProperty("app")]
        public App app { get; set; }
        public string token { get; set; }
        public string hashed_token { get; set; }
        public string token_last_eight { get; set; }
        public string note { get; set; }
        public string note_url { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        [JsonProperty("scopes")]
        public string[] scopes { get; set; }
        public string fingerprint { get; set; }
    }
}
