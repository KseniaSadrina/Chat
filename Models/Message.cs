using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
	public class Message
	{
		[JsonProperty("id")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[JsonProperty("sender")]
		public string Sender { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("sessionName")]
		public string SessionName { get; set; }

		[JsonProperty("timestamp")]
		public DateTime TimeStamp { get; set; }

	}
}
