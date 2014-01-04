using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Othello.Models
{
    public class UserRecord
    {
        [Key] // primary key
        public int Id { get; set; }
        [Required] // not null
        public DateTime TimeStamp { get; set; }
        public string Name { get; set; }

        public UserRecord()
        {
            TimeStamp = DateTime.Now;
        }
    }
}