using Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
	public class Training
	{

		[JsonProperty("id")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("scenario")]
		public virtual Scenario Scenario { get; set; }

		[JsonProperty("state")]
		public TrainingState State { get; set; }

	}
}
