using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BaphoDashBoard.DTO
{
    public class ChartDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public int Value { get; set; }
        [JsonProperty("porcent")]
        public int Porcent { get; set; }
    }
}
