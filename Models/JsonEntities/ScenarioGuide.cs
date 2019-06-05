using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ScenarioGuide
    {
        [JsonProperty("scenario_name")]
        public string ScenarioName { get; set; }

        [JsonProperty("scenario_id")]
        public int ScenarioId { get; set; }

        [JsonProperty("abstract")]
        public string Abstract { get; set; }

        [JsonProperty("goals")]
        public IList<GoalGuide> Goals { get; set; }
    }
}
