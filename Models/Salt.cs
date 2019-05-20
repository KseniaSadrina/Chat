using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class Salt
    {
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public string Secret { get; set; }
    }
}
