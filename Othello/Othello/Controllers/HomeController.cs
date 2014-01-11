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
        private static DataContext data = new DataContext();

        //private static int t = 0;

        //public HomeController()
        //    : base()
        //{
        //    t++;
        //}

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StartGame(Player newPlayer)
        {
            data.Players.Add(newPlayer);
            return CheckForPlaymate(newPlayer);
        }

        private Player findPlaymate(Player actPlayer)
        {
            IQueryable<Player> players = data.Players.Where(p => p.State == PlayerState.Waiting && p.Id != actPlayer.Id).OrderBy(p => p.LastUpdate);
            DateTime stamp = DateTime.UtcNow;
            bool ch = false; // detect change
            Player result = null;
            try
            {
                foreach (Player p in players)
                    if ((stamp - p.LastUpdate).TotalSeconds > 10) // timeout players
                    {
                        p.State = PlayerState.Disconnected; ch = true;
                    }
                    else result = p;
                return result;
            }
            finally
            {
                if (ch) data.SaveChanges();
            }
        }

        public ActionResult CheckForPlaymate(Player player)
        {
            player = data.Players.Find(player.Id); // refresh
            // check if this player wasn't already selected as playmate
            if (player.State==PlayerState.Playing)
                return View("Game", player); 

            player.LastUpdate = DateTime.UtcNow;
            try
            {
                // find playmate
                Player playmate = findPlaymate(player);
                if (playmate != null)
                {
                    player.State = PlayerState.Playing;
                    playmate.State = PlayerState.Playing;
                    return View("Game", player);
                }
                else // playmate wasn't found - wait for it
                    return View("WaitForPlaymate", player);
            }
            finally
            {
                data.SaveChanges();
            }
        }
    }
}
