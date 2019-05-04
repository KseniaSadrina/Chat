using Models.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Models
{
	// Chat session is kind of a conversation
	// We have a new conversation per each session with 
	public class ChatSession
	{
		public ChatSession()
		{
			Users = new List<SessionUser>();
		}

		[JsonProperty("id")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		public ChannelType Type { get; set; }

		[JsonProperty("trainingId")]
		[ForeignKey(nameof(Training))]
		public int TrainingId { get; set; }

		// many to many relationship mapped by a joined table
		public virtual ICollection<SessionUser> Users { get; set; }

		public virtual ICollection<Message> Messages { get; set; }

	}
}
