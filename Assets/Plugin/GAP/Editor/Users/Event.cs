using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Plugins.GAP.Editor.Users
{
    public class Actor: ScriptableObject
    {
        public long id { get; set; }
        public string login { get; set; }
        public string display_login { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string avatar_url { get; set; }
    }
    public class Repo: ScriptableObject
    {
        public long id { get; set; }
        [JsonProperty("name")]
        public string _name { get; set; }
        public string url { get; set; }
    }

    public class Member : ScriptableObject
    {
        public string login { get; set; }
        public long id { get; set; }
        public string node_id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public bool site_admin { get; set; }
    }

    public class Author: ScriptableObject
    {
        public string email { get; set; }
        [JsonProperty("name")]
        public string _name { get; set; }
    }

    public class Commits : ScriptableObject
    {
        public string sha { get; set; }
        public Author author { get; set; }
        public string message { get; set; }
        public bool distinct { get; set; }
        public string url { get; set; }
    }

    public class Payload
    {
        public long push_id { get; set; }
        public long size { get; set; }
        [JsonProperty("ref")]
        public string _ref { get; set; }
        public string ref_type { get; set; }
        public string master_branch { get; set; }
        public string description { get; set; }
        public string pusher_type { get; set; }
        public string head { get; set; }
        public string before { get; set; }
        [JsonProperty("member")]
        public Member member { get; set; }
        [JsonProperty("commits")]
        public Commits[] commits { get; set; }
        public string action { get; set; }
    }

    public class Event : ScriptableObject
    {
        public string id { get; set; }
        public string type { get; set; }
        [JsonProperty("actor")]
        public Actor actor { get; set; }
        [JsonProperty("repo")]
        public Repo repo { get; set; }
        [JsonProperty("payload")]
        public Payload payload { get; set; }
        [JsonProperty("public")]
        public bool _public { get; set; }
        public string created_at { get; set; }
    }
}
