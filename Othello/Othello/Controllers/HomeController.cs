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

        private static object lockObject = new object();

        #region Game start

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StartGame(Player newPlayer)
        {
            using (DataContext data = new DataContext())
            {
                data.Players.Add(newPlayer);
                data.SaveChanges();
                return RedirectToAction("CheckForPlaymate", new { idPlayer = newPlayer.Id });
            }
        }

        #endregion

        #region Game over

        public ActionResult GameOver(int idGame, int idPlayer)
        {
            using (DataContext data = new DataContext())
            {
                return View(data.ConstructPlayerGame(idGame, idPlayer));
            }
        }

        public ActionResult PlayAgain(int idPlayer)
        {
            using (DataContext data = new DataContext())
            {
                Player p = data.Players.Find(idPlayer);
                p.State = PlayerState.Waiting;
                p.LastUpdate = DateTime.UtcNow;
                data.SaveChanges();
                return RedirectToAction("CheckForPlaymate", new { idPlayer = p.Id });
            }
        }

        #endregion

        #region Game

        public ActionResult Game(int idGame, int idPlayer)
        {
            PlayerGame pg;
            using (DataContext data = new DataContext())
            {
                pg = data.ConstructPlayerGame(idGame, idPlayer);
            }
            // game over check
            if (pg.GameState.IsGameOver())
                return RedirectToAction("GameOver", new { idGame = pg.GameState.idBlackPlayer, idPlayer = pg.Player.Id });
            else
                return View(pg);
        }

        public JsonResult checkGame(int idGame, int idPlayer)
        {
            using (DataContext data = new DataContext())
            {
                GameState gs = data.FetchAdditional(data.GameStates.Find(idGame));
                if (gs == null) throw new Exception(string.Format("checkGame - Game with id {0} doesn't exists", idGame));
                Player p = gs.PlayerByID(idPlayer);
                if (p==null) throw new Exception(string.Format("checkGame - Player with id {0} doesn't play game {1}", idPlayer, idGame));
                p.LastUpdate = DateTime.UtcNow; p.State = PlayerState.Playing;
                gs.WhitePlayer.CheckTimeout();
                gs.BlackPlayer.CheckTimeout();
                data.SaveChanges();
                Player o = (gs.WhitePlayer.Id == idPlayer ? gs.BlackPlayer : gs.WhitePlayer ); // oponent
                // result
                return Json(new { 
                    ImPlaying = gs.ActivePlayerInstance.Id==idPlayer , 
                    OpponentTimeout = o.State==PlayerState.Disconnected
                }, JsonRequestBehavior.AllowGet);
            }  
        }

        public ActionResult GameTurn(int idG, int idP, int x, int y)
        {
            using (DataContext data = new DataContext())
            {
                GameState gameState = data.FetchAdditional(data.GameStates.Find(idG));
                if (gameState == null) throw new Exception(string.Format("Game with id {0} doesn't exists", idG));
                if (gameState.ActivePlayerInstance.Id != idP) throw new Exception("GameTurn - Wrong player ID");
                gameState.NextTurn(x, y);
                data.PushGameStateToHistory(gameState);
                data.SaveChanges();
            }
            return RedirectToAction("Game", new { idGame = idG, idPlayer = idP });
        }

        #endregion

        #region Waiting for playmate

        public JsonResult checkPlayer(int idPlayer)
        {
            using (DataContext data = new DataContext())
            {
                Player p = data.Players.Find(idPlayer);
                if (p != null && !p.IsDisconnected)
                {
                    p.LastUpdate = DateTime.UtcNow;
                    data.SaveChanges();
                }
                return Json(p, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckForPlaymate(int idPlayer)
        {
            lock (lockObject) // serialization
            {
                using (DataContext data = new DataContext())
                {
                    Player player = data.Players.Find(idPlayer); // refresh
                    if (player == null) throw new Exception(string.Format("Player with id {0} doesn't exists", idPlayer));
                    // check if this player wasn't already selected as playmate
                    if (player.State == PlayerState.Playing)
                    {
                        IQueryable<GameState> games = data.GameStates.Where(gs => gs.Invalid == false && (gs.WhitePlayer.Id == player.Id || gs.BlackPlayer.Id == player.Id)).OrderByDescending(gs => gs.TimeStamp);
                        GameState valid = null;
                        foreach (GameState g in games)
                        {
                            if (valid == null) valid = g;
                            else g.Invalid = true;
                        }
                        if (valid == null) throw new Exception(string.Format("Game is missing for player (id={0), nick={1}", player.Id, player.Name));
                        data.SaveChanges();
                        return RedirectToAction("Game", new { idGame = valid.Id, idPlayer = player.Id });
                    }

                    player.LastUpdate = DateTime.UtcNow;

                    // find playmate
                    Player playmate = data.FindPlaymate(player);
                    if (playmate != null)
                    {
                        player.State = PlayerState.Playing;
                        playmate.State = PlayerState.Playing;
                        // create game
                        GameState gs = new GameState(player, playmate);
                        data.GameStates.Add(gs);
                        data.PushGameStateToHistory(gs);
                        data.SaveChanges();
                        return RedirectToAction("Game", new { idGame = gs.Id, idPlayer = player.Id });
                    }
                    else // playmate wasn't found - wait for it
                        return View("WaitForPlaymate", player);
                }
            }
        }
        
        #endregion

    }
}
