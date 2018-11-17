
using farmarproject2.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Threading.Tasks;

namespace farmarproject2.Controllers
{
    public class SericeController : Controller
    {
        // GET: Serice
        public ActionResult Index()
        {
            TempData.Keep();
            return View();
        }

        public ActionResult SArea1()
        {
            return PartialView("_SArea1");
        }

        public ActionResult SArea2()
        {
            return PartialView("_SArea2");
        }

        public ActionResult SArea3()
        {
            return PartialView("_SArea3");
        }

        public ActionResult SArea4()
        {
            return PartialView("_SArea4");
        }

        public ActionResult SArea5()
        {
            return PartialView("_SArea6");
        }

        //GET
        public ActionResult SAreaDT1()
        {
            global::farmarproject2.Models.farmarEntities1 farmar = new Models.farmarEntities1();
            var List2 = farmar.ServiceLogins.Select(x => x).ToList();

            var List1 = List2.Select(x => new ServiceLogin2
            {
                userID = x.userID,
                State = x.State,
                Type = x.Type,
                LoginTime = x.LoginTime.ToString(),
                ContentA = x.ContentA
            }).ToList<ServiceLogin2>();
            return Json(new { data = List1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SAreaDT2()
        {
            global::farmarproject2.Models.farmarEntities1 farmar = new Models.farmarEntities1();
            var List2 = farmar.AspNetUsers.Select(x => x).ToList();

            var List1 = List2.Select(x => new Serviceuser
            {
                Email = x.Email,
                FamName = x.FamName,
                PhoneNumber = x.PhoneNumber,
                UserIg = x.UserIg,
                EmailA = x.Id
            }).ToList<Serviceuser>();
            return Json(new { data = List1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SAreaDT3()
        {
            global::farmarproject2.Models.farmarEntities1 farmar = new Models.farmarEntities1();
            var List2 = farmar.products.Select(x => x).ToList();

            var List1 = List2.Select(x => new Serviceitem
            {
                productid = x.productid,
                productname = x.productname,
                unitprice = x.unitprice,
                unitstock = x.unitstock,
                //description = $"{x.description.Substring(0,15)}.....",
                picture = x.productid,
                category = x.category,
                user_email = x.user_email,
                EmailA = x.productid
            }).ToList<Serviceitem>();
            foreach (var x in List1)
            {
                var i = List2.Where(o => o.productid == x.productid).SingleOrDefault();
                if (i.description.Length < 16)
                    x.description = i.description;
                else
                    x.description = $"{i.description.Substring(0, 15)}.....";
            }
            return Json(new { data = List1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SAreaDT4()
        {
            global::farmarproject2.Models.farmarEntities1 farmar = new Models.farmarEntities1();
            var List2 = farmar.orders.Select(x => x).ToList();


            var List1 = List2.Select(x => new Serviceordetail
            {
                order_id = x.order_id,
                buy_Name = x.buy_id,
                sell_id= x.sell_id,
                total_price= farmar.order_detail.Where(o => o.order_id == x.order_id).Select(p=>p.total_price).Sum(),
                status=x.status,
                 date= x.date,
                EmailA= x.order_id
            }).ToList<Serviceordetail>();
            foreach (var x in List1)
            {
                var T = farmar.order_detail.Where(o => o.order_id == x.order_id).ToList();
                foreach (var y in T)
                {
                    x.detail += $"<tr><td>商品名稱:{y.productname}</td><td>數量:{y.quiantity}</td><td>金額:{y.total_price}</td></tr>";
                }
            }
            return Json(new { data = List1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SAreaDT5()
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
           return Json( List1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SAreaDTP(string kind)
        {
            int[] array1 = new int[] { 0, 0, 0, 0};


            array1[0] = chartkinduse(kind, "07");
            array1[1] = chartkinduse(kind, "08");
            array1[2] = chartkinduse(kind, "09");
            array1[3] = chartkinduse(kind, "10");
            var List1 = array1.ToList();
            return Json(List1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SAreaDT7()
        {
            int[] array1 = new int[] { 0, 0, 0, 0 ,0};
            string[] array2 = new string[] { "", "", "", "" ,""};
            var i = 0;
            farmarEntities1 farmar = new farmarEntities1();
            var l1= farmar.products.OrderByDescending(o => o.sale).Take(5);
            foreach (var x in l1)
            {
                array1[i] = (int)(x.sale);
                array2[i] = x.productname;
                i++;
            }


           
            return Json(new {array1= array1, array2=array2 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Itemphoto(int id)
        {
            global::farmarproject2.Models.farmarEntities1 farmar = new Models.farmarEntities1();
            var x = farmar.products.Find(id).picture;
            return File(x, "image/jpeg");
        }

        public ActionResult MemberEditView(string id = "")
        {
            if (id == "")
            {
                return View("Index");
            }
            global::farmarproject2.Models.farmarEntities1 farmar = new Models.farmarEntities1();
            AspNetUser user = farmar.AspNetUsers.Where(o => o.Id == id).SingleOrDefault();
            return View(user);
        }

        public ActionResult ItemEditView(int id = 0)
        {
            if (id == 0)
            {
                return View("Index");
            }
            global::farmarproject2.Models.farmarEntities1 farmar = new Models.farmarEntities1();
            product user = farmar.products.Where(o => o.productid == id).SingleOrDefault();
            return View(user);
        }

        public ActionResult OrderEditView(int id = 0)
        {
            if (id == 0)
            {
                return View("Index");
            }
            global::farmarproject2.Models.farmarEntities1 farmar = new Models.farmarEntities1();
            Serviceorder user = farmar.orders.Where(o => o.order_id == id).Select(o => new Serviceorder
            {
               order_id= o.order_id,
                status= o.status,
                buy_id= o.buy_id,
                flag=true
            }).SingleOrDefault();

            var T= farmar.order_detail.Where(o => o.order_id == id).ToList();
            foreach (var x in T)
            {
                var produ = farmar.products.Where(o => o.productid == x.productid).SingleOrDefault();
                if (produ.unitstock < x.quiantity)
                {
                    user.flag = false;
                }
            }
            
            return View(user);
        }

        public ActionResult DeleteItemView(int id = 0)
        {
            if (id == 0)
            {
                return View("Index");
            }
            global::farmarproject2.Models.farmarEntities1 farmar = new Models.farmarEntities1();
            product user = farmar.products.Where(o => o.productid == id).SingleOrDefault();
            return View(user);
        }

        public ActionResult AddItemView()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem([Bind(Include = "productid,productname,unitprice,unitstock,description,picture,picture1,picture2,category,category_multiple,sell_id")] product product)
        {
            farmarEntities1 db = new farmarEntities1();
            if (ModelState.IsValid)
            {
                if (Request.Files["File1"] != null)
                {
                    byte[] data = null;

                    using (BinaryReader br = new BinaryReader(Request.Files["File1"].InputStream))
                    {

                        data = br.ReadBytes(Request.Files["File1"].ContentLength);


                    }
                    product.picture = data;
                }
                ServiceLogin service = new ServiceLogin()
                {
                    userID = "famar123",
                    LoginTime = DateTime.Now,
                    ContentA = "新增商品",
                    State = "新增",
                    Type = "商品資料"
                };
                db.ServiceLogins.Add(service);
                product.user_email = "管理員";
                product.sale = 0;
                db.products.Add(product);
                db.SaveChanges();

                return Json("OK");
            }
            return Json("NO");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(AspNetUser user, HttpPostedFileBase File1 = null)
        {
            farmarEntities1 db = new farmarEntities1();
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
                    tempuser.UserIg = user.UserIg;
                    TempName.name = user.UserIg;
                }

                db.Entry(tempuser).State = EntityState.Modified;

                ServiceLogin service = new ServiceLogin()
                {
                    userID = "famar123",
                    LoginTime = DateTime.Now,
                    ContentA = $"{user.Email}會員的資料更改",
                    State = "修改",
                    Type = "會員資料"
                };
                db.ServiceLogins.Add(service);
                db.SaveChanges();

                TempName.username = user.FamName;
                return Json("yes");
            }
            return HttpNotFound();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteItem(product product)
        {
            farmarEntities1 db = new farmarEntities1();

            product tempuser = db.products.Where(o => o.productid == product.productid).Select(p => p).SingleOrDefault();
            ServiceLogin service = new ServiceLogin()
            {
                userID = "famar123",
                LoginTime = DateTime.Now,
                ContentA = $"編號{product.productid}商品-刪除",
                State = "刪除",
                Type = "商品資料"
            };
            db.ServiceLogins.Add(service);
            db.products.Remove(tempuser);
            db.SaveChanges();
            return Json("yes");
        }

        public ActionResult ResetPsView(string id = "")
        {
            if (id == "")
            {
                return View("Index");
            }
            global::farmarproject2.Models.farmarEntities1 farmar = new Models.farmarEntities1();
            AspNetUser user = farmar.AspNetUsers.Where(o => o.Id == id).SingleOrDefault();
            return View(user);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> ResetPs(AspNetUser user)
        {
            if (ModelState.IsValid)
            {
                string password = "Fm!";
                string[] p1 = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
                string[] p2 = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                Random random = new Random();
                for (int i = 0; i <= 4; i++)
                {
                    var temp = random.Next(0, 2);
                    var temp2 = random.Next(0, 10);
                    if (temp == 0)
                    {
                        password = $"{password}{p1[temp2]}";
                    }
                    else
                    {
                        password = $"{password}{p2[temp2]}";
                    }
                }
                Crypto.HashPassword(password);
                farmarEntities1 farmar = new farmarEntities1();
                AspNetUser tempuser = farmar.AspNetUsers.Where(o => o.Email == user.Email).SingleOrDefault();
                tempuser.PasswordHash = Crypto.HashPassword(password);
                farmar.Entry(tempuser).State = EntityState.Modified;

                List<string> mailname = new List<string>();
                mailname.Add(user.Email);
   
                await SendMailByGmail(mailname, "碼農是好農密碼重設通知", MailMessage("密碼已重設", $"您的碼農是好農帳戶新密碼為:{password}", user.FamName));
                ServiceLogin service = new ServiceLogin()
                {
                    userID = "famar123",
                    LoginTime = DateTime.Now,
                    ContentA = $"{user.Email}會員的密碼更改",
                    State = "修改",
                    Type = "會員資料"
                };
                farmar.ServiceLogins.Add(service);
                farmar.SaveChanges();
                HttpCookie Cookie = new HttpCookie(".AspNet.ApplicationCookie");
                Cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(Cookie);
                return Json("ok");
            }
            return HttpNotFound();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ItemEdit(product product)
        {
            farmarEntities1 db = new farmarEntities1();
            product tempuser = db.products.Where(o => o.productid == product.productid).Select(p => p).SingleOrDefault();
            if (ModelState.IsValid)
            {
                if (Request.Files["File1"].FileName != "")
                {
                    byte[] data = null;
                    using (BinaryReader br = new BinaryReader(Request.Files["File1"].InputStream))
                    {
                        data = br.ReadBytes(Request.Files["File1"].ContentLength);
                    }
                    tempuser.picture = data;
                }
                else
                {
                    tempuser.picture = product.picture;

                }
                if (product.category != null)
                {
                    tempuser.category = product.category;
                }
                tempuser.productname = product.productname;
                tempuser.unitprice = product.unitprice;
                tempuser.unitstock = product.unitstock;
                tempuser.description = product.description;
                tempuser.user_email = product.user_email;

                db.Entry(tempuser).State = EntityState.Modified;

                ServiceLogin service = new ServiceLogin()
                {
                    userID = "famar123",
                    LoginTime = DateTime.Now,
                    ContentA = $"編號{product.productid}商品-資料更改",
                    State = "修改",
                    Type = "商品資料"
                };
                db.ServiceLogins.Add(service);
                db.SaveChanges();

                return Json("yes");
            }
            return HttpNotFound();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OrderEdit(Serviceorder od)
        {
            farmarEntities1 db = new farmarEntities1();
            order tempuser = db.orders.Where(o => o.order_id == od.order_id).Select(p => p).SingleOrDefault();
            string produt = "~";
            if (ModelState.IsValid)
            {
                if (od.status != null)
                {
                    tempuser.status = od.status;
                }
                if (od.status == "付款成功")
                {
                    tempuser.date = DateTime.Now.ToString();
                }
                db.Entry(tempuser).State = EntityState.Modified;

                ServiceLogin service = new ServiceLogin()
                {
                    userID = "famar123",
                    LoginTime = DateTime.Now,
                    ContentA = $"編號{od.order_id}定單-資料更改",
                    State = "修改",
                    Type = "訂單資料"
                };
                db.ServiceLogins.Add(service);

                var T = db.order_detail.Where(o => o.order_id == od.order_id).ToList();
                if (od.status == "付款成功")
                {
                    foreach (var x in T)
                    {
                        var pt = db.products.Where(a => a.productid == x.productid).FirstOrDefault();
                        if (pt.unitstock >= x.quiantity)
                        {
                            pt.unitstock = pt.unitstock - x.quiantity;
                        }
                        pt.sale += x.quiantity;
                        db.Entry(pt).State = EntityState.Modified;
                    }
                }
                else if (od.status == "缺貨中")
                {
                    foreach (var x in T)
                    {
                        var pt = db.products.Where(a => a.productid == x.productid).FirstOrDefault();
                        if (pt.unitstock < x.quiantity)
                        {
                            
                           
                                produt = $"{produt}{pt.productname}~";
                           
                        }
                    }
                }
                List<string> MailList = new List<string>();
                MailList.Add(od.buy_id);
                var name = db.AspNetUsers.Where(o => o.Email == od.buy_id).Select(a => a.FamName).FirstOrDefault();
                db.SaveChanges();
                if (od.status == "付款成功")
                {
                   await SendMailByGmail(MailList, "碼農是好農交易成功通知", MailMessage("交易明細", $"您的訂單編號{od.order_id}已經完成交易", name));
                }
                else if(od.status=="缺貨中")
                {
                   await SendMailByGmail(MailList, "碼農是好農缺貨通知", MailMessage("缺貨通知", $"您的訂單編號{od.order_id},商品{produt}庫存不足已取消訂單,待庫存足夠請重新下單", name));
                }
                    return Json("yes");
            }
            return HttpNotFound();
        }

        //GET
        public ActionResult Logout()
        {
            global::farmarproject2.Models.farmarEntities1 farmar = new farmarEntities1();
            ServiceLogin service = new ServiceLogin()
            {
                userID = "famar123",
                LoginTime = DateTime.Now,
                ContentA = $"famar123登出",
                State = "登出",
                Type = "用戶"
            };
            if (Request.Cookies["ServerID"] != null)
            {
                HttpCookie Cookie = new HttpCookie("ServerID");
                Cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(Cookie);
            }
            farmar.ServiceLogins.Add(service);
            farmar.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        private async Task SendMailByGmail(List<string> MailList, string Subject, string Body)
        {
            MailMessage msg = new MailMessage();
            //收件者，以逗號分隔不同收件者 ex "test@gmail.com,test2@gmail.com"
            msg.To.Add(string.Join(",", MailList.ToArray()));
            msg.From = new MailAddress("msit119test@gmail.com", "碼農管理員", System.Text.Encoding.UTF8);
            //郵件標題 
            msg.Subject = Subject;
            //郵件標題編碼  
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            //郵件內容
            msg.Body = Body;
            msg.IsBodyHtml = true;
            msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
            msg.Priority = MailPriority.Normal;//郵件優先級 
                                               //建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port 
            #region 其它 Host
            /*
             *  outlook.com smtp.live.com port:25
             *  yahoo smtp.mail.yahoo.com.tw port:465
            */
            #endregion
            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);
            //設定你的帳號密碼
            MySmtp.Credentials = new System.Net.NetworkCredential("msit119test@gmail.com", "lkk123456");
            //Gmial 的 smtp 使用 SSL
            MySmtp.EnableSsl = true;
            await  MySmtp.SendMailAsync(msg);
        }

        public string MailMessage(string title, string content, string name)
        {
            string t = $"<table border='0' cellpadding='0' cellspacing='0' height='100%' style='min-width:348px' width='100%'><tbody><tr height='32px'></tr><tr align='center'><td><table border='0' cellpadding='0' cellspacing='0' style='max-width:600px'><tbody><tr><td><table bgcolor='#D94235' border='0' cellpadding='0' cellspacing='0' style='min-width:332px;max-width:600px;border:1px solid #f0f0f0;border-bottom:0;border-top-left-radius:3px;border-top-right-radius:3px'width='100%'><tbody><tr><td colspan='3' height='72px'></td></tr><tr><td width='32px'></td><td style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:24px;color:#ffffff;line-height:1.25;min-width:300px'>{title}</td><td width='32px'></td></tr><tr><td colspan='3' height='18px'></td></tr></tbody></table></td></tr><tr><td><table bgcolor='#FAFAFA' border='0' cellpadding='0' cellspacing='0' style='min-width:332px;max-width:600px;border:1px solid #f0f0f0;border-bottom:1px solid #c0c0c0;border-top:0;border-bottom-left-radius:3px;border-bottom-right-radius:3px' width='100%'><tbody><tr height='16px'><td rowspan='3' width='32px'></td><td></td><td rowspan='3' width='32px'></td></tr><tr><td><table border='0' cellpadding='0' cellspacing='0' style='min-width:300px'><tbody><tr><td style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:13px;color:#202020;line-height:1.5;padding-bottom:4px'>{name}您好:</td></tr><tr><td style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:13px;color:#202020;line-height:1.5;padding:4px 0'>{content}</td></tr><tr><td style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:13px;color:#202020;line-height:1.5;padding-top:28px'>碼農是好農敬上</td></tr><tr height='16px'></tr><tr><td><table style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:12px;color:#b9b9b9;line-height:1.5'><tbody><tr><td>請勿回覆這封電子郵件。如需詳細資訊，請聯絡管理員</td></tr></tbody></table></td></tr></tbody></table></td></tr><tr height='32px'></tr></tbody></table></td></tr><tr height='16'></tr><tr><td style='max-width:600px;font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:10px;color:#bcbcbc;line-height:1.5'></td></tr></tbody></table></td></tr></tbody></table>"; 
            return  t;
        }

        private int chartkinduse(string dkind,string mon)
        {
            int sum = 0;
            farmarEntities1 farmar = new farmarEntities1();
            var q = farmar.orders.Where(o => o.status == "付款成功");
            var oid1 = (from s in q
                        where s.date.Substring(5, 2) ==mon
                        select s.order_id).ToList();
            foreach (var x in oid1)
            {
                var l1 = farmar.order_detail.Where(a => a.order_id == x).ToList();
                foreach (var y in l1)
                {
                    var w = farmar.products.Where(z => y.productid == z.productid).FirstOrDefault();
                    if (w.category == dkind)
                    {
                        sum += y.quiantity;
                    }
                }
            }
            return sum;
        }
    }
}