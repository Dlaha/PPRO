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
        public DbSet<Player> Players { get; set; }
        public DbSet<GameState> GameStates { get; set; }

        public DataContext():base()
        {
            Configuration.LazyLoadingEnabled = true;
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<GameState>().HasOptional(gs => gs.Previous).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<GameState>().HasRequired(gs => gs.WhitePlayer).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<GameState>().HasRequired(gs => gs.BlackPlayer).WithMany().WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }

        public GameState FetchPlayers(GameState gamestate)
        {
            gamestate.BlackPlayer = Players.Find(gamestate.idBlackPlayer);
            gamestate.WhitePlayer = Players.Find(gamestate.idWhitePlayer);
            return gamestate;
        }

        public T FetchPlayers<T>(T gamestates) where T : IEnumerable<GameState>
        {
            foreach (GameState item in gamestates)
                FetchPlayers(item);
            return gamestates;
        }
    }
}