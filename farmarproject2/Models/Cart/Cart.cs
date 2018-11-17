using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using farmarproject2.Models;

namespace Carts.Models.Cart
{
    [Serializable] //可序列化
    public class Cart : IEnumerable<CartItem> //購物車類別
    {
        //建構值
        public Cart()
        {
            this.cartItems = new List<CartItem>();
        }

        //儲存所有商品
        private List<CartItem> cartItems;

        /// <summary>
        /// 取得購物車內商品的總數量
        /// </summary>
        public int Count
        {
            get
            {
                return this.cartItems.Count;
            }
        }

        //取得商品總價
        public decimal TotalAmount
        {
            get 
            {
                decimal totalAmount = 0.0m;
                foreach(var cartItem in this.cartItems)
                {
                    totalAmount = totalAmount + cartItem.Amount;
                }
                return totalAmount;
            }
        }

        //新增一筆Product，使用ProductId
        public bool AddProduct(int ProductId,int quantity)
        {
            var findItem = this.cartItems
                            .Where(s => s.Id == ProductId)
                            .Select(s => s)
                            .FirstOrDefault();

            //判斷相同Id的CartItem是否已經存在購物車內
            if (findItem == default(Models.Cart.CartItem))
            {   //不存在購物車內，則新增一筆
                using( farmarproject2.Models.farmarEntities1 db = new farmarEntities1() )
                {
                    var product = (from s in db.products 
                                  where s.productid == ProductId 
                                  select s).FirstOrDefault();
                    if( product != default( farmarproject2.Models.product ) )
                    {
                         
                        this.AddProduct(product, quantity);
                    }
                }             
            }
            else
            {   //存在購物車內，則將商品數量累加
                findItem.Quantity = findItem.Quantity+ quantity;
            }
            return true;
        }

        //新增一筆Product，使用Product物件
        private bool AddProduct(product product,int quantity)
        {
            //將Product轉為CartItem
            var cartItem = new Models.Cart.CartItem()
            {
                Id = product.productid,
                Name = product.productname,
                Price = product.unitprice,
                Quantity = quantity,
                sell_id = product.user_email,
                category=product.category
            };

            //加入CartItem至購物車
            this.cartItems.Add(cartItem);
            return true;
        }

        //移除一筆Product，使用ProductId
        public bool RemoveProduct(int ProductId)
        {
            var findItem = this.cartItems
                            .Where(s => s.Id == ProductId)
                            .Select(s => s)
                            .FirstOrDefault();

            //判斷相同Id的CartItem是否已經存在購物車內
            if (findItem == default(Models.Cart.CartItem))
            {   
                //不存在購物車內，不需做任何動作
            }
            else
            {   //存在購物車內，將商品移除
                this.cartItems.Remove(findItem);
            }
            return true;
        }

        public bool Removesell_id(string sell_id)
        {
            var findItem = this.cartItems
                            .Where(s => s.sell_id == sell_id)
                            .Select(s => s).ToList();
                            

            //判斷相同Id的CartItem是否已經存在購物車內
            if (findItem == null)
            {
                //不存在購物車內，不需做任何動作
            }
            else
            {   //存在購物車內，將商品移除


                foreach (var item in findItem)
                {
                    this.cartItems.Remove(item); 
                }
            }
            return true;
        }

        //清空購物車
        public bool ClearCart()
        {
            this.cartItems.Clear();
            return true;
        }
        public List<order_detail> ToOrderDetailList(int orderId)
        {
            var result = new List<order_detail>();
            foreach (var cartItem in this.cartItems)
            {
                result.Add(new order_detail()
                {
                    productid = cartItem.Id,
                    productname = cartItem.Name,
                    total_price = cartItem.Amount,
                    quiantity = cartItem.Quantity,
                    order_id=orderId,
                    sell_id=cartItem.sell_id,
                    status = "尚未出貨",
                    category =cartItem.category
                    
                });
            }
            return result;
        }

        #region IEnumerator

        IEnumerator<CartItem> IEnumerable<CartItem>.GetEnumerator()
        {
            return this.cartItems.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.cartItems.GetEnumerator();
        }

        #endregion
    }
}


