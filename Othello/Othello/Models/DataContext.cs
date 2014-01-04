using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Othello.Models
{
    public class DataContext:DbContext
    {
        public DbSet<UserRecord> TimeStamps { get; set; }

    }
}