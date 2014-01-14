using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Othello.Models
{
    public enum PlayerState
    {
        Waiting,
        Playing,
        Disconnected
    }

    public class Player
    {
        [Key] // primary key
        public int Id { get; set; }
        [Required] // not null
        public DateTime WaitStartTime { get; set; }
        [Required] // not null
        public string Name { get; set; }
        [Required]
        public PlayerState State { get; set; }
        public DateTime LastUpdate { get; set; } // last interaction from client side


        public Player()
        {
            WaitStartTime = DateTime.UtcNow;
            LastUpdate = WaitStartTime;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}",Id,Name);
        }
    }
}