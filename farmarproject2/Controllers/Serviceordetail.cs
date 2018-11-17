using System;
using System.Collections.Generic;
using farmarproject2.Models;

namespace farmarproject2.Controllers
{
    internal class Serviceordetail
    {
        public int order_id { get; set; }
        public string buy_Name { get; set; }
        public string sell_id { get; set; }
        public decimal total_price { get; set; }
        public string status { get; set; }
        public string detail { get; set; }
        public int EmailA { get; set; }
        public string date { get; set; }
    }
}