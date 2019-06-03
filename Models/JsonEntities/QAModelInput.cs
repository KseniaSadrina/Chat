using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class QAModelInput
    {
        [JsonProperty("question")]
        public string Question { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
