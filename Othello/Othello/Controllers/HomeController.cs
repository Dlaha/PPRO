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
            return View("WaitForPlaymate", newPlayer);
            //return RedirectToAction("WaitForPlaymate",newPlayer);
        }
    }
}
