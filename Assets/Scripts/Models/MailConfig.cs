using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aci.Unity.Models
{
    class MailConfig
    {
        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("recipient")]
        public List<string> Recipient { get; set; }

        [JsonProperty("copy")]
        public List<string> Copy { get; set; }

        [JsonProperty("blindcopy")]
        public List<string> Blindcopy { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("servername")]
        public string Servername { get; set; }

        [JsonProperty("port")]
        public string Port { get; set; }

        [JsonProperty("ssl")]
        public bool SSL { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
