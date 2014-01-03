using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Othello.Models
{
    public class TimeStampRecord
    {
        [Key]
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }

        public TimeStampRecord()
        {
            TimeStamp = DateTime.Now;
        }
    }

    public class DataContext:DbContext
    {
        public DbSet<TimeStampRecord> TimeStamps { get; set; }

    }
}