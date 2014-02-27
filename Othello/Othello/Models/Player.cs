using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public const int TimeOutTime = 10; // [s]

        [Key] // primary key
        public int Id { get; set; }
        [Required] // not null
        public DateTime WaitStartTime { get; set; }
        [Required] // not null
        public string Name { get; set; }
        [Required]
        public PlayerState State { get; set; }
        public DateTime LastUpdate { get; set; } // last interaction from client side

        public string WaitingTime
        {
            get
            {
                TimeSpan d = LastUpdate - WaitStartTime;
                return string.Format("{0}:{1:00}", (int)d.TotalMinutes, d.Seconds);
            }
        }

        [NotMapped]
        public bool IsPlaying {
            get { return State == PlayerState.Playing; }
        }

        [NotMapped]
        public bool IsDisconnected {
            get { return State == PlayerState.Disconnected; }
        }

        public void CheckTimeout()
        {
            if ((DateTime.UtcNow - LastUpdate).TotalSeconds > Player.TimeOutTime)
                State = PlayerState.Disconnected;
        }

        public Player()
        {
            WaitStartTime = DateTime.UtcNow;
            LastUpdate = WaitStartTime;
            State = PlayerState.Waiting;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}",Id,Name);
        }
    }
}