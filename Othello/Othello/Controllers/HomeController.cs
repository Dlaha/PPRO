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

        #region Game

        public ActionResult Game(int idGame, int idPlayer)
        {
            GameState gameState;
            using (DataContext data = new DataContext())
            {
                gameState = data.FetchAdditional(data.GameStates.Find(idGame));
            }
            return View(gameState);
        }

        public ActionResult GameTurn(int idG, int idP, int x, int y)
        {
            using (DataContext data = new DataContext())
            {
                GameState gameState = data.FetchAdditional(data.GameStates.Find(idG));
                if (gameState.ActivePlayerInstance.Id != idP) throw new Exception("GameTurn - Wrong player ID");
                gameState.NextTurn(x, y);
                data.SaveChanges();
            }
            return RedirectToAction("Game", new { idGame = idG });
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
            using (DataContext data = new DataContext())
            {
                Player player = data.Players.Find(idPlayer); // refresh
                if (player == null) throw new Exception(string.Format("Player with id {0} doesn't exists",idPlayer));
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
                    data.SaveChanges();
                    return RedirectToAction("Game", new { idGame = gs.Id, idPlayer = player.Id });
                }
                else // playmate wasn't found - wait for it
                    return View("WaitForPlaymate", player);
            }
        }

        #endregion

    }
}
