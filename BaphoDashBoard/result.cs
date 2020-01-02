using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRI_Inquiry
{
    public abstract class ResultBase
    {
        [JsonProperty("status")]
        public int status { get; set; }
        [JsonProperty("success")]
        public bool success { get; set; } = true;
        [JsonProperty("message")]
        public string message { get; set; }
    }

    public class SRIResult<T> : ResultBase where T : class
    {
        public SRIResult()
        {
            if (typeof(T) != typeof(String))
                MRObject = (T)Activator.CreateInstance(typeof(T));
        }

        [JsonProperty("mr_object")]
        public T MRObject { get; set; }
    }

    public class SRIResult : ResultBase
    {
        public SRIResult()
        {
        }
    }
}
