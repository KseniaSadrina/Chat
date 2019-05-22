

using Newtonsoft.Json;

namespace Models
{
    public class GoalGuide
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("goal_name")]
        public string GoalName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
