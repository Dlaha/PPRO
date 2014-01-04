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
            return View(data.TimeStamps.ToList());
        }

        public ActionResult Remember()
        {
            return Remember("NULL");
        }

        [HttpPost]
        public ActionResult Remember(string name)
        {
            data.TimeStamps.Add(new UserRecord() { Name = name });
            data.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
