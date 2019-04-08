using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
	public class SessionUser
	{
		public int UserId { get; set; }
		public User User { get; set; }
		public int SessionId { get; set; }
		public ChatSession Session { get; set; }
	}
}
