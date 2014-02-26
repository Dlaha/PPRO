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
        public DbSet<GameHistory> History { get; set; }

        public DataContext():base()
        {
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameState>().HasRequired(gs => gs.WhitePlayer).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<GameState>().HasRequired(gs => gs.BlackPlayer).WithMany().WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }

        public GameState FetchAdditional(GameState gamestate)
        {
            if (gamestate == null) return null;
            gamestate.BlackPlayer = Players.Find(gamestate.idBlackPlayer);
            gamestate.WhitePlayer = Players.Find(gamestate.idWhitePlayer);
            return gamestate;
        }

        public T FetchPlayers<T>(T gamestates) where T : IEnumerable<GameState>
        {
            foreach (GameState item in gamestates)
                FetchAdditional(item);
            return gamestates;
        }

        public Player FindPlaymate(Player actPlayer)
        {
            Player result = null;
            IQueryable<Player> players = Players.Where(p => p.State == PlayerState.Waiting && p.Id != actPlayer.Id).OrderBy(p => p.WaitStartTime);
            foreach (Player p in players)
            {
                p.CheckTimeout(); // timeout players
                if (!p.IsDisconnected) result = p; // select longest waiting playmate
            }
            SaveChanges();
            return result;
        }

        public PlayerGame ConstructPlayerGame(int idGame, int idPlayer)
        {
            GameState gameState;
            gameState = FetchAdditional(GameStates.Find(idGame));
            if (gameState == null)
                throw new Exception(string.Format("ConstructPlayerGame - Game with id {0} doesn't exists", idGame));
            if (gameState.BlackPlayer.Id != idPlayer && gameState.WhitePlayer.Id != idPlayer)
                throw new Exception(string.Format("ConstructPlayerGame - GamePlayer with id {0} doesn't play game {1}", idPlayer, idGame));
            return new PlayerGame((idPlayer == gameState.BlackPlayer.Id ? gameState.BlackPlayer : gameState.WhitePlayer), gameState);
        }
    }
}