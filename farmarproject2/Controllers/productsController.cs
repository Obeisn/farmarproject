using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using farmarproject2.Models;
using Microsoft.AspNet.Identity;

namespace farmarproject2.Controllers
{
    public class productsController : Controller
    {
        private farmarEntities1 db = new farmarEntities1();

        // GET: products
        public ActionResult Index()
        {
            ViewBag.category = new SelectList(db.products, "productid", "category");
            return View(db.products);
          
        }
    
        public ActionResult showcate(string category)
        {
            var c = db.products.Where(a => a.category == category).ToList();
            return View(c);
        }

        // GET: products/Details/5     
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Addcomment(int productid, string content)
        {
            var userId =User.Identity.Name;
            var comment = new Models.comment()
            {
                ProductID = productid,
                Contents = content,
                UserId = userId,
                CreateDate = DateTime.Now
            };
            using (farmarEntities1 db = new farmarEntities1())
            {
                db.comments.Add(comment);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id =  productid});
        }

        [Authorize]
        // GET: products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "productid,productname,unitprice,unitstock,description,picture,picture1,picture2,category,category_multiple,sell_id")] product product)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files["File1"] != null)
                {
                    byte[] data = null;
                   
                    using (BinaryReader br = new BinaryReader(Request.Files["File1"].InputStream))
                    {

                        data = br.ReadBytes(Request.Files["File1"].ContentLength);
                       
                       
                    }
                    //using (BinaryReader br1 = new BinaryReader(Request.Files["File2"].InputStream))
                    //{
                    //    data2 = br1.ReadBytes(Request.Files["File2"].ContentLength);
                    //}
                    //using (BinaryReader br2 = new BinaryReader(Request.Files["File3"].InputStream))
                    //{
                    //    data3 = br2.ReadBytes(Request.Files["File3"].ContentLength);
                    //}
                    product.picture = data;
                    //product.picture1 = data2;
                    //product.picture2 = data3;
                }              
                product.user_email = User.Identity.Name;
                product.sale = 0;
                db.products.Add(product);
                db.SaveChanges();
                if (product.category_multiple=="是")
                {
                    return RedirectToAction("show", "products", null);
                }
                return RedirectToAction("homepage","Home",null);
            }
            return View(product);
        }

        // GET: products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "productid,productname,unitprice,unitstock,description,picture,category,category_multiple,sell_id")] product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            product product = db.products.Find(id);
            db.products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public FileResult showphoto(int id)
        {
            farmarEntities1 db = new farmarEntities1();
            byte[] data = db.products.Find(id).picture;
            return File(data, "image/jpeg");
        }
      
        public ActionResult show()
        {
            var query = db.products.Where(x => x.category_multiple == "是").Select(x => x).OrderByDescending(x=>x.productid);
            return View(query);
        }

        public ActionResult photo(int id)
        {
            var x = db.products.Find(id).picture;
            return File(x, "image/jpeg");
        }

        public ActionResult getproductchart()
        {
            var sum = 0;
            float[] array1 = new float[] { 0, 0, 0, 0, 0 };
            farmarEntities1 farmar = new farmarEntities1();
            var Oid = farmar.orders.Where(o => o.status == "付款成功").Select(z => z.order_id);
            foreach (var x in Oid)
            {
                var l1 = farmar.order_detail.Where(a => a.order_id == x).ToList();

                foreach (var y in l1)
                {
                    var q = farmar.products.Where(z => y.productid == z.productid).Select(o => o.category).FirstOrDefault();
                    switch (q)
                    {
                        case "肉類":
                            array1[0] += y.quiantity;
                            break;
                        case "海鮮":
                            array1[1] += y.quiantity;
                            break;
                        case "蔬果":
                            array1[2] += y.quiantity;
                            break;
                        case "調味品":
                            array1[3] += y.quiantity;
                            break;
                        case "五穀雜糧":
                            array1[4] += y.quiantity;
                            break;
                    }
                    sum += y.quiantity;
                }
            }
            for (int i = 0; i < array1.Length; i++)
            {
                array1[i] = array1[i] / sum * 100;
            }
            var List1 = array1.ToList();
            return Json(List1, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
