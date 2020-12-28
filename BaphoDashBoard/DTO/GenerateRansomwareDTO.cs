using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaphoDashBoard.DTO
{
    public class GenerateRansomwareDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("hosts")]
        public string Hosts { get; set; }
        [JsonProperty("hosts_list")]
        public ArrayList HostsList = new ArrayList();
    }
}
