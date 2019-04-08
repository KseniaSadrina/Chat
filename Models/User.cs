using Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
	public class User
	{
		public User()
		{
			Sessions = new List<SessionUser>();
		}

		[JsonProperty("id")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[JsonProperty("sender")]
		[NotMapped]
		public string FullName => String.Join(" ", FirstName, LastName);

		[JsonProperty("firstName")]
		public string FirstName { get; set; }

		[JsonProperty("lastName")]
		public string LastName { get; set; }

		[JsonProperty("useName")]
		public string UserName { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("sessions")]
		public virtual ICollection<SessionUser> Sessions { get; set; }

		[JsonProperty("type")]
		public UserType UserType { get; set; }

	}
}
