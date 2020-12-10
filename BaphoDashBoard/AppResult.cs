using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaphoDashBoard
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

    public class AppResult<T> : ResultBase where T : class
    {
        public AppResult()
        {
            if (typeof(T) != typeof(String))
                MRObject = (T)Activator.CreateInstance(typeof(T));
        }

        [JsonProperty("mr_object")]
        public T MRObject { get; set; }
    }

    public class AppResult : ResultBase
    {
        public AppResult()
        {
        }
    }
}
