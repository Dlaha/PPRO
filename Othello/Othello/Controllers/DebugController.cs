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
            return View(data.Players.ToList());
        }

    }
}
