using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Othello.Models
{
    public class Player
    {
        [Key] // primary key
        public int Id { get; set; }
        [Required] // not null
        public DateTime TimeStamp { get; set; }
        [Required] // not null
        public string Name { get; set; }

        public Player()
        {
            TimeStamp = DateTime.Now;
        }
    }
}