using System.ComponentModel.DataAnnotations;

namespace farmarproject2.Models
{
    public class productMetadata
    {
       [Required(ErrorMessage ="{0}為必填")]
       [Display(Name = "產品名稱")]
        public string productname { get; set; }
        [Display(Name = "產品單價")]
        [DisplayFormat(DataFormatString ="{0:C}")]
        public decimal unitprice { get; set; }
        [Display(Name = "數量")]    
        public int unitstock { get; set; }
        [Display(Name = "產品描述")]
        public string description { get; set; }
        [Display(Name = "產品照片")]
        public byte[] picture { get; set; }
        [Display(Name = "種類")]
        public string category { get; set; }
        [Display(Name = "是否為團購??")]
        public string category_multiple { get; set; }
        [Display(Name = "編號")]
        public int productid { get; set; }
        [Display(Name = "發佈人")]
        public string user_email { get; set; }
    }
}