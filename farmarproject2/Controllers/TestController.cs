using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Carts.Controllers
{
    public class TestController : Controller
    {
        public ActionResult GetCart(int amount)
        {
            //取得目前購物車
            var cart = Models.Cart.Operation.GetCurrentCart();
            cart.AddProduct(1,amount);          

            //回傳目前購物車中的商品總價
            return Content( string.Format("目前購物車總共: {0}元", cart.TotalAmount) );
        }

        public ActionResult TestBootStrap()
        {
            return View();
        }
    }
}