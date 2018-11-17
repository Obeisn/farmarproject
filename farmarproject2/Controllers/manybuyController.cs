using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using farmarproject2.Models;

namespace farmarproject2.Controllers
{
    public class manybuyController : Controller
    {

       farmarEntities1 db = new farmarEntities1();
        // GET: manybuy
        public ActionResult Index(int? id)
        {
            var x = db.products.Find(id);
            ViewBag.description = x.description;
            ViewBag.category = x.category;
            ViewBag.productname = x.productname;

            var y = db.products.Find(id).productid;
            ViewBag.id = id;
            ViewBag.pictire = x.picture;
            ViewBag.nowday = DateTime.Now;
            ViewBag.product = y;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "multi_buy_name,raiser_id,productid,raise_time,deadline,maxamount")]multi_buy multi_Buy)
        {
            var s = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
            if (ModelState.IsValid)
            {           
                db.multi_buy.Add(multi_Buy);
                db.SaveChanges();
            }
            return RedirectToAction("showmanybuy", "manybuy");
        }

        public ActionResult showphoto(int id)
        {
            var s = db.products.Find(id).picture;
            return File(s, "image/*");
        }

        public ActionResult showmanybuy()
        {
            var s = db.multi_buy.OrderByDescending(x=>x.productid);
            return View(s);
        }

       public ActionResult showdetial(int id)
        {
            var produntid = db.multi_buy.Find(id);
            var s = produntid.productid;
            ViewBag.proddescript = db.products.Where(x => x.productid == s).Select(x => x.description).Single();
            ViewBag.produntid = s;
            ViewBag.deadline = produntid.deadline;

            var amount = db.multi_buy_list.Where(x => x.multi_buy_id == id).Select(x => x.amount).ToArray();
            if (amount != null)
            {
                var ag = amount.Sum();
                ViewBag.currentamount = ag;
            }
            else
            {
                ViewBag.currentamount = 0;
            }

            var content = db.multi_buy.Find(id);
            ViewBag.multi_buy_name = content.multi_buy_name;
            ViewBag.id = content.multi_buy_id;

            return View(content);
        }
  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult showdetial([Bind(Include = "multi_buy_id,join_id,amount,deadine")]multi_buy_list mbl)
        {
            if(ModelState.IsValid)
            {
                //db.currentstore(mbl.multi_buy_id, mbl.amount);    
                db.multi_buy_list.Add(mbl);
                db.SaveChanges();
            }
            return RedirectToAction("showmanybuy");
        }

        public ActionResult usermanybuy()
        {
            var self = db.multi_buy.Where(x => x.raiser_id == User.Identity.Name).Select(x => x);
            return PartialView("_usermanybuy", self);
        }
        public ActionResult userjoin()
        {
            var now = DateTime.Now;
            var s = db.multi_buy_list.Where(x => x.deadine > now).Select(x => x).OrderByDescending(x=>x.multi_buy_list_id);

            //db.multi_buy.Where(x=>x.multi_buy_id=s)

            return PartialView("_userjoin", s);
        }

        public ActionResult userdelete(int id)
        {
            var s = db.multi_buy.Find(id);
            db.multi_buy.Remove(s);
            db.SaveChanges();

            var self = db.multi_buy.Where(x => x.raiser_id == User.Identity.Name).Select(x => x);
            return PartialView("_usermanybuy", self);

        }

        public ActionResult userjoindelete(int id)
        {
            var now = DateTime.Now;
            var sm = db.multi_buy_list.Find(id);
            db.multi_buy_list.Remove(sm);
            db.SaveChanges();

            var s = db.multi_buy_list.Where(x => x.deadine > now).Select(x => x).OrderByDescending(x=>x.multi_buy_list_id);

            //db.multi_buy.Where(x=>x.multi_buy_id=s)

            return PartialView("_userjoin", s);
        }
    }
}