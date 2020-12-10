using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaphoDashBoard.DTO
{
    public class VictimDetailsDTO
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("hostname")]
        public string Hostname { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("loc")]
        public string Localitation { get; set; }
        [JsonProperty("postal")]
        public string PostalCode { get; set; }
        [JsonProperty("machine_name")]
        public string MachineName { get; set; }
        [JsonProperty("machine_Os")]
        public string MachineOs { get; set; }
    }
}
