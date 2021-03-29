using Newtonsoft.Json;

namespace Aci.Unity.Models
{
    class SmsConfig
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("port")]
        public string Port { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }
    }
}
