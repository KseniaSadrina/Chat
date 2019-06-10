using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class Goal
    {
        [JsonProperty("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("scenarioId")]
        [ForeignKey(nameof(Scenario))]
        public int ScenarioId { get; set; }

        [JsonIgnore]
        public virtual IList<TrainingGoal> TrainingGoals { get; set; }

        public int Order { get; set; }

    }
}
