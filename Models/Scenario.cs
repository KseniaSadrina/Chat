using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
	public class Scenario
	{
        public Scenario()
        {
            Goals = new List<Goal>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
        public string Description { get; set; }

        public virtual IList<Goal> Goals { get; set; }

	}
}
