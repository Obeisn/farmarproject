using farmarproject2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace farmarproject2.Controllers
{
    public class cookbookController : Controller
    {
        farmarEntities1 db = new farmarEntities1();

        // GET: cookbook
        public ActionResult Index()
        {

            var s = db.cookbooks.Select(x => x);
            return View(s);
        }

        public ActionResult search(string id)
        {
            string text = "";
            List<product> dd = new List<product>();
            var oo = db.products.Select(x => x.productname).ToList();
            for (int i = 0; i < oo.Count(); i++)
            {
                text += oo[i];
            }
            var s = db.products.Select(x => x.productname).ToList();
            if (text.Contains(id))
            {
                foreach (var y in s)
                {
                    if (y.Contains(id))
                    {
                        dd.AddRange(db.products.Where(x => x.productname == y).Select(x => x));
                    }
                }
                return PartialView("_search", dd);
            }
            else
            {
                return PartialView("_nosearch");
            }
        }

        public ActionResult showcookbook(int? id)
        {
            var x = db.cookbooks.Select(y => y.cookid).ToList();
            var cook = db.cookbooks.Select(y => y).OrderByDescending(y => y.cookid).Take(3);
            ViewBag.nearcook = cook;
            var s = db.cookbooks.Find(id);

            return View(s);
        }
    }
}