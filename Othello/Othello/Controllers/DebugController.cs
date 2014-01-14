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
        private DataContext data = new DataContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Players()
        {
            List<Player> players = data.Players.ToList();
            return View(players);
        }

        public ActionResult GameStates()
        {
            List<GameState> gs = data.FetchPlayers(data.GameStates.ToList());      
            return View(gs);
        }

    }
}
