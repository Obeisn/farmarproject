using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace farmarproject2.Controllers
{
    public class foodController : Controller
    {
        // GET: food
        public ActionResult Index()
        {
            return View();
        }
        public RedirectToRouteResult goback()
        {
            return RedirectToAction("Index","Home",null);
        }
    }
}