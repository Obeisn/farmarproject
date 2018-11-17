
using farmarproject.metadata;
using farmarproject2.Controllers;
using farmarproject2.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace farmarproject.Controllers
{
    public class UserAccountController : Controller
    {
        farmarEntities1 db = new farmarEntities1();

        // GET: UserAccount
        public ActionResult Index()
        {
            var s = db.AspNetUsers.Where(x => x.UserName == User.Identity.Name).Select(x => x.Id).Single();
            var models = db.AspNetUsers.Where(x => x.UserName == User.Identity.Name).Select(x => x).SingleOrDefault();
            ViewBag.id = s;
            return View(models);
        }

        public ActionResult showuserdetial(string id)
        {
            var userid = db.AspNetUsers.Find(id);

            return PartialView("_userdatialPart", userid);
        }

        public ActionResult resetpassword(string id)
        {
            return PartialView("_resetpasswordPartial");
        }

        public ActionResult userporduct()
        {
            var s = db.products.Where(x => x.user_email == User.Identity.Name).Select(x => x);
            return View("SoldDetails", s);
        }

        public ActionResult joindetial(int id)
        {
            var s = db.multi_buy_list.Where(x => x.multi_buy_id == id).Select(x => x);
            return PartialView("_joindetial", s);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(AspNetUser user, HttpPostedFileBase File1)
        {
            if (ModelState.IsValid)
            {
                AspNetUser tempuser = db.AspNetUsers.Where(o => o.Email == user.Email).Select(p => p).SingleOrDefault();
                tempuser.FamName = user.FamName;
                tempuser.PhoneNumber = user.PhoneNumber;

                var tempimg = "";
                //換圖
                if (File1 != null)
                {
                    string filejpg = Path.GetExtension(File1.FileName);
                    tempimg = $"{user.Email}{filejpg}";
                    string routeimg = Path.Combine(Server.MapPath("~/image"), $"{user.Email}{filejpg}");
                    File1.SaveAs(routeimg);
                    tempuser.UserIg = tempimg;
                    TempName.name = tempimg;
                }
                else
                {
                    tempuser.UserIg = "No-image-available.jpg";
                    TempName.name = "No-image-available.jpg";
                }
                //
                db.Entry(tempuser).State = EntityState.Modified;
                db.SaveChanges();

                TempName.username = user.FamName;
                return RedirectToAction("Index", "Home");
            }
            return HttpNotFound();
        }

        // 商品清單JSON
        public ActionResult SoldDetailsJSON()
        {
            var email = User.Identity.GetUserName();
            var n = db.products.Where(c1 => c1.user_email == email).Select(e => e).ToList();
            var products = n.Select((c2, Index) => new soldProject()
            {
                Number = Index + 1,
                productid = c2.productid,
                productname = c2.productname,
                category = c2.category,
                unitprice = c2.unitprice,
                unitstock = c2.unitstock,
                description = c2.description,
            }).ToList();

            return Json(new { data = products }, JsonRequestBehavior.AllowGet);
        }

        // 顯示商品圖片
        public FileResult ShowPicture(int id)
        {
            var content = db.products.Where(c => c.productid == id).SingleOrDefault().picture;
            if (content != null)
            {
                return File(content, "image/jpeg");
            }
            else
            {
                return null;
            }
        }

        // GET: products/Details/5
        public ActionResult SoldDetails()
        {
            var email = User.Identity.GetUserName();
            if (email == "")            // email為string，用""
            {
                // 重新導向，用RedirectToAction
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult FormEdit(int productid)
        {
            ViewBag.Action = "FormEdit";
            return View(db.products.Where(c => c.productid == productid).ToList().Select(c => new soldProject
            {
                category = c.category,
                productname = c.productname,
                unitprice = c.unitprice,
                unitstock = c.unitstock,
                description = c.description,
                picture = c.picture,
            }).Single());
        }

        [HttpPost]
        public void FormEdit([Bind(Include = "productid, category, productname, unitprice, unitstock, description, picture")]soldProject model)
        {
            if (ModelState.IsValid)
            {
                product EditRecord = db.products.Where(c => c.productid == model.productid).Single();
                EditRecord.category = model.category;
                EditRecord.productname = model.productname;
                EditRecord.unitprice = model.unitprice;
                EditRecord.unitstock = model.unitstock;
                EditRecord.description = model.description;
                EditRecord.picture = model.picture;
                db.Entry(EditRecord).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public ActionResult FormCreate()
        {
            ViewBag.Action = "FormCreate";
            soldProject dt = new soldProject();
            return View("FormEdit", dt);
        }

        [HttpPost]
        public void FormCreate([Bind(Include = "category, productname, unitprice, unitstock, description, picture")]soldProject model)
        {
            if (ModelState.IsValid)
            {
                product InsertRecord = new product();
                InsertRecord.category = model.category;
                InsertRecord.productname = model.productname;
                InsertRecord.unitprice = model.unitprice;
                InsertRecord.unitstock = model.unitstock;
                InsertRecord.description = model.description;
                InsertRecord.picture = model.picture;
                db.products.Add(InsertRecord);
                db.SaveChanges();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormDelete(int[] IDChecked)
        {
            db.products.RemoveRange(db.products.Where(c => IDChecked.Contains(c.productid)));
            db.SaveChanges();
            return RedirectToAction("SoldDetails");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ItemEditView(int id=0 )
        {
            if (id == 0)
            {
                return View("Index");
            }
            global::farmarproject2.Models.farmarEntities1 farmar = new farmarEntities1();
            product user = farmar.products.Where(o => o.productid == id).SingleOrDefault();
            return View(user);
        }
    }
}