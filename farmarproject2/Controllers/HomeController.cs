using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using farmarproject2.Models;
using AllPay.Payment.Integration;

namespace farmarproject2.Controllers
{
    public class HomeController : Controller
    {
        farmarEntities1 db = new farmarEntities1();
        public ActionResult homepage()
        {
            var ad = db.advertisings.Select(x => x).ToList();
            ViewBag.ad = ad;

            var products = db.products.ToList();
            return View(products);
        }

        public ActionResult help() {
            return View();
        }

        public ActionResult photo(int id)
        {
           
                var x = db.products.Find(id).picture;
                return File(x, "image/jpeg"); 
             
           
        }
       
        public ActionResult photoproduct(int id)
        {
            var a = db.products.Where(c => c.productid == id && c.category_multiple=="否").ToArray();
            byte[] b = a.Select(c => c.picture).FirstOrDefault();

            return File(b, "image/jpeg");
        }

        public ActionResult showcate(string category)
        {
            
            var c = db.products.Where(a => a.category == category).Select(a=>a);
            if (c.Count()==0)
            {
                return PartialView("_noitems");
            }
            else
            {
                return PartialView("_showcate", c);
            }            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}