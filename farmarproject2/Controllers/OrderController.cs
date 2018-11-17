using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using farmarproject2.Models;
using Microsoft.AspNet.Identity;
using Carts.Models.Cart;
using System.Data.Entity;
using AllPay.Payment.Integration;

namespace farmarproject.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ship postback,string sell_id) {
            if (this.ModelState.IsValid)
            {   //取得目前購物車
                var currentcart = Carts.Models.Cart.Operation.GetCurrentCart();

                //取得目前登入使用者Id
                var userId =User.Identity.GetUserName();

                using (farmarEntities1 db = new farmarEntities1())
                {
                    //建立Order物件
                    order order = new order()
                    {
                        buy_id = userId,
                        buy_Name = postback.buy_Name,
                        buy_Phone = postback.buy_phone,
                        buy_Address = postback.buy_Address,
                        order_category = "非預購",
                        build_time = DateTime.Now,
                         sell_id=sell_id,
                        status = "未付款",                        
                    };
                    //加其入Orders資料表後，儲存變更
                    db.orders.Add(order);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {

                        throw ex;
                    }
                    //取得購物車中OrderDetai物件
                    var orderDetails = currentcart.ToOrderDetailList(order.order_id);
                    var orders = from c in orderDetails
                                 where c.sell_id == sell_id 
                                 select c;
                    //將其加入OrderDetails資料表後，儲存變更
                    db.order_detail.AddRange(orders);
                    
                    db.SaveChanges();
                    var currentCart =Operation.GetCurrentCart();
                    currentCart.Removesell_id(sell_id);
                 
                }
                return RedirectToAction("Orders");
            }
            return View();
        }

        public ActionResult Orders() {
            farmarEntities1 db = new farmarEntities1();
            var orders = from c in db.orders
                    where c.buy_id == User.Identity.Name
                         select c;
            return View(orders);
        }

        public ActionResult OrderAllSeller()
        {
            return View();
        }

        public ActionResult SearchOrderBySell()
        {
            farmarEntities1 db = new farmarEntities1();
            var orders = from c in db.orders
                         where c.sell_id == User.Identity.Name
                         select c;
            return View(orders);
        }

        public ActionResult Pay(Int32 order_id)
        {
            farmarEntities1 db = new farmarEntities1();
            var orderdetail = (from m in db.orders
                               where m.order_id ==order_id
                               select m).FirstOrDefault();
            string result = ProcessPayment(orderdetail);

            return Content(result);
        }

        public string ProcessPayment(order orderdetail)
        {
            List<string> enErrors = new List<string>();
            decimal x = 0;
            var y = 0;
            string szHtml = String.Empty;
            try
            {
                using (AllInOne oPayment = new AllInOne())
                {
                    /* 服務參數 */
                    oPayment.ServiceMethod = HttpMethod.HttpPOST;
                    oPayment.ServiceURL = "https://payment-stage.allpay.com.tw/Cashier/AioCheckOut/V2";
                    oPayment.HashKey = "5294y06JbISpM5x9";
                    oPayment.HashIV = "v77hoKGq4kWxNNIS";
                    oPayment.MerchantID = "2000132";

                    /* 基本參數 */
                    string hostname = this.Request.Url.Authority;
                    oPayment.Send.ReturnURL = $"http://{hostname}/AllPayPayment/CallBack";
                    oPayment.Send.MerchantTradeNo = DateTime.Now.ToString("yyyyMMddHHmmss");
                    oPayment.Send.MerchantTradeDate = DateTime.Now;
                    //oPayment.Send.TotalAmount = 1;
                    oPayment.Send.TradeDesc = "測試金流的描述 ABC";
                    var details = orderdetail.order_detail.Select(a => a).ToList();
                    foreach (var item in details)
                    {
                       
                        oPayment.Send.Items.Add(new Item
                        {
                           Name=item.productname,
                           Price=item.total_price,
                           Quantity=item.quiantity
                          

                        });
                        y = item.quiantity; 
                         x += item.total_price;
                    }
                    oPayment.Send.TotalAmount = x*y;
                    //oPayment.Send.Items.Add(new Item
                    //{
                    //    Name = $"向{orderdetail.sell_id}下定的東西",

                    //});



                    /* 產生訂單 */
                    enErrors.AddRange(oPayment.CheckOut());
                    /* 產生產生訂單 Html Code 的方法 */
                    enErrors.AddRange(oPayment.CheckOutString(ref szHtml));

                }
            }
            catch (Exception ex)
            {
                // 例外錯誤處理。 
                enErrors.Add(ex.Message);
            }
            finally
            {
                // 顯示錯誤訊息。 
                if (enErrors.Count() > 0)
                {
                    szHtml = String.Join("\\r\\n", enErrors);
                }
                orderdetail.status = "完成交易";
            }
            return szHtml;
        }

        public ActionResult minuscate(int order_id)
        {
            farmarEntities1 db = new farmarEntities1();
        
            order o = db.orders.Where(a => a.order_id == order_id).Select(b => b).SingleOrDefault();
            o.date = DateTime.Now.ToString();
            o.status = "付款成功";           
            db.Entry(o).State = EntityState.Modified;
            db.SaveChanges();
          
            return null;
        }
    }
}