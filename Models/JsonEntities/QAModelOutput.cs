using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.JsonEntities
{
    public class QAModelOutput
    {
        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("score")]
        public float Score { get; set; }
    }
}
