using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Othello.Models;

namespace Othello.Controllers
{
    public class HomeController : Controller
    {
        private DataContext data = new DataContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StartGame(Player newPlayer)
        {
            data.Players.Add(newPlayer);
            data.SaveChanges();
            return RedirectToAction("CheckForPlaymate", new { idPlayer = newPlayer.Id });
        }

        private Player findPlaymate(Player actPlayer)
        {
            IQueryable<Player> players = data.Players.Where(p => p.State == PlayerState.Waiting && p.Id != actPlayer.Id).OrderBy(p => p.WaitStartTime);
            DateTime stamp = DateTime.UtcNow;
            bool ch = false; // detect change
            Player result = null;
            foreach (Player p in players)
                if ((stamp - p.LastUpdate).TotalSeconds > 10) // timeout players
                {
                    p.State = PlayerState.Disconnected; ch = true;
                }
                else result = p; // select longest waiting playmate
            if (ch) data.SaveChanges();
            return result;
        }

        public ActionResult Game(int idGame)
        {
            GameState gameState = data.FetchPlayers(data.GameStates.Find(idGame));
            return View(gameState);
        }

        public ActionResult CheckForPlaymate(int idPlayer)
        {
            Player player = data.Players.Find(idPlayer); // refresh
            // check if this player wasn't already selected as playmate
            if (player.State == PlayerState.Playing)
            {
                IQueryable<GameState> games = data.GameStates.Where(gs => gs.Previous == null && gs.Invalid == false && (gs.WhitePlayer.Id == player.Id || gs.BlackPlayer.Id == player.Id)).OrderByDescending(gs => gs.TimeStamp);
                GameState valid = null;
                foreach (GameState g in games)
                {
                    if (valid == null) valid = g;
                    else g.Invalid = true;
                }
                if (valid == null) throw new Exception(string.Format("Game is missing for player (id={0), nick={1}", player.Id, player.Name));
                data.SaveChanges();
                return RedirectToAction("Game", new { idGame = valid.Id });
            }

            player.LastUpdate = DateTime.UtcNow;

            // find playmate
            Player playmate = findPlaymate(player);
            if (playmate != null)
            {
                player.State = PlayerState.Playing;
                playmate.State = PlayerState.Playing;
                // create game
                GameState gs = new GameState(player, playmate);
                data.GameStates.Add(gs);
                data.SaveChanges();
                return RedirectToAction("Game", new { idGame = gs.Id });
            }
            else // playmate wasn't found - wait for it
                return View("WaitForPlaymate", player);
        }
    }
}
