using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AllPay.Payment.Integration;
using farmarproject2.Models;

namespace farmarproject2.Controllers
{
    public class order_detailController : Controller
    {
        private farmarEntities1 db = new farmarEntities1();

        // GET: order_detail
        public ActionResult Index()
        {
            var order_detail = db.order_detail.Include(o => o.order);
            return View(order_detail.ToList());
        }

        // GET: order_detail/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            order_detail order_detail = db.order_detail.Find(id);
            if (order_detail == null)
            {
                return HttpNotFound();
            }
            return View(order_detail);
        }

        // GET: order_detail/Create
        public ActionResult Create()
        {
            ViewBag.order_id = new SelectList(db.orders, "order_id", "buy_Name");
            return View();
        }

        // POST: order_detail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "order_detail_id,order_id,productname,quiantity,build_time,total_price,productid,buy_id")] order_detail order_detail)
        {
            if (ModelState.IsValid)
            {
                db.order_detail.Add(order_detail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.order_id = new SelectList(db.orders, "order_id", "buy_Name", order_detail.order_id);
            return View(order_detail);
        }

        // GET: order_detail/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            order_detail order_detail = db.order_detail.Find(id);
            if (order_detail == null)
            {
                return HttpNotFound();
            }
            ViewBag.order_id = new SelectList(db.orders, "order_id", "buy_Name", order_detail.order_id);

            return View(order_detail);
        }

        // POST: order_detail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "order_detail_id,order_id,productname,quiantity,build_time,total_price,productid,buy_id")] order_detail order_detail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order_detail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.order_id = new SelectList(db.orders, "order_id", "buy_Name", order_detail.order_id);
            return View(order_detail);
        }

        // GET: order_detail/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            order_detail order_detail = db.order_detail.Find(id);
            if (order_detail == null)
            {
                return HttpNotFound();
            }
            return View(order_detail);
        }

        // POST: order_detail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            order_detail order_detail = db.order_detail.Find(id);
            db.order_detail.Remove(order_detail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: order_detail/Edit/5
        public ActionResult search_order_detail(int? id)
        {
            var orderdetail = from m in db.order_detail
                              where m.order_id == id
                              select m;

            return View(orderdetail);
        }

        public ActionResult search_order_detail_sell(int? id)
        {
            var orderdetail = from m in db.order_detail
                              where m.order_id == id
                              select m;

            return View(orderdetail);
        }

       
        public ActionResult LogisticsReturn()
        {
            string content = "";
            foreach (string key in this.Request.Form.AllKeys)
            {
                string value = this.Request.Form[key];
                content += $"{key}={value}<br/>";
            }
            return Content(content);
        }
        public ActionResult pack(int? order_detail_id,int? product_id, int quintity)
        {
            farmarEntities1 db = new farmarEntities1();
            product c = db.products.Where(a => a.productid == product_id).Select(b => b).SingleOrDefault();
            c.unitstock -= quintity;
            c.sale += quintity;
            db.Entry(c).State = EntityState.Modified;
            db.SaveChanges();
            order_detail o = db.order_detail.Where(a => a.order_detail_id == order_detail_id).Select(b => b).SingleOrDefault();
            o.status = "成功出貨";
            db.SaveChanges();
            return RedirectToAction("Index", "Home",null);
        }
    }
}
