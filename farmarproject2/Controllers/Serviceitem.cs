namespace farmarproject2.Controllers
{
    internal class Serviceitem
    {
        public int productid { get; set; }
        public string productname { get; set; }
        public decimal unitprice { get; set; }
        public int unitstock { get; set; }
        public string description { get; set; }
        public int picture { get; set; }
        public string category { get; set; }
        public string user_email { get; set; }
        public int EmailA { get; internal set; }
    }
}