using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Othello.Models;

namespace Othello.Controllers
{
    public class DebugController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Players()
        {
            List<Player> players;
            using (DataContext data = new DataContext())
            {
                players = data.Players.ToList();
            }
            return View(players);
        }

        public ActionResult GameStates()
        {
            List<GameState> gs;
            using (DataContext data = new DataContext())
            {
                gs = data.FetchPlayers(data.GameStates.ToList());
            }
            return View(gs);
        }

        public ActionResult GameHistory(int? idGameState)
        {
            List<GameHistory> gh;
            using (DataContext data = new DataContext())
            {
                if (!idGameState.HasValue)
                    gh = data.History.OrderBy(h => new { h.idGameState, h.TimeStamp }).ToList();
                else
                    gh = data.History.Where(h => h.idGameState == idGameState.Value).OrderBy(h => h.TimeStamp).ToList();
            }
            return View(gh);
        }
    }
}
