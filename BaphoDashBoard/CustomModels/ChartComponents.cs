using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BaphoDashBoard.CustomModels.CssNames;

namespace BaphoDashBoard.CustomModels
{
    public class ChartComponents
    {
        public class VMChartDatasetBase
        {
            [JsonProperty("label")]
            public string Label { get; set; }

            [JsonProperty("use_color")]
            public bool UseColor { get; set; } = false;

            [JsonProperty("color")]
            public string Color { get; set; } = BackgroundColorClasses.GlowGreen;

            [JsonProperty("border")]
            public string Border { get; set; } = BorderColorClasses.GlowGreen;

        }

        //Q clase de pqerias
        public class VMChartDataset : VMChartDatasetBase
        {
            [JsonProperty("data")]
            public List<object> Data { get; set; } = new List<object>();
        }

        //Fatal
        public class VMChartDataset<T> : VMChartDatasetBase //maybe no necesito esto
        {
            [JsonProperty("data")]
            public List<T> Data { get; set; } = new List<T>();
        }

        public class VMChartBase
        {
            [JsonProperty("labels")]
            public List<string> Labels { get; set; } = new List<string>(); //Esto yo creo q esta maaaaal

            [JsonProperty("tooltips")]
            public List<string> Description { get; set; } = new List<string>();

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("multiple_dataset")]
            public bool MultipleDataset { get; set; } = true;

            [JsonProperty("legend")]
            public bool Legend { get; set; } = false;
        }


        public class VMChart : VMChartBase
        {
            [JsonProperty("dataset")]
            public List<VMChartDataset> DataSet { get; set; } = new List<VMChartDataset>();
        }


        public class VMChart<T> : VMChartBase
        {
            [JsonProperty("dataset")]
            public List<VMChartDataset<T>> DataSet { get; set; } = new List<VMChartDataset<T>>();
        }
    }
}
