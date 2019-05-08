using Microsoft.AspNetCore.Identity;
using Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class User : IdentityUser<int>
    {
		public User()
		{
			Sessions = new List<SessionUser>();
		}

        [JsonProperty("fullName")]
		[NotMapped]
		public string FullName => String.Join(" ", FirstName, LastName);

		[JsonProperty("firstName")]
		public string FirstName { get; set; }

		[JsonProperty("lastName")]
		public string LastName { get; set; }

        [JsonProperty("sessions")]
		public virtual ICollection<SessionUser> Sessions { get; set; }

		[JsonProperty("type")]
		public UserType UserType { get; set; }

	}
}
