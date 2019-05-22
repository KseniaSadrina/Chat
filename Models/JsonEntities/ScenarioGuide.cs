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
        public string ScenarioId { get; set; }

        [JsonProperty("goals")]
        public IList<GoalGuide> Goals { get; set; }
    }
}
