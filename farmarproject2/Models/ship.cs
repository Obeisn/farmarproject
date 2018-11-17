using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace farmarproject2.Models
{
    public class ship
    {
      [Required]
      [Display(Name ="姓名")]
      [StringLength(10,ErrorMessage ="{0}至少要{2}個字",MinimumLength =2)]

        public string buy_Name { get; set; }
        [Required]
        [Display(Name ="聯絡電話")]
        [StringLength(15,ErrorMessage = "{0}至少要{2}個字",MinimumLength =8)]
        public string buy_phone { get; set; }
        [Required]
        [Display(Name = "收貨人地址")]
        [StringLength(15, ErrorMessage = "{0}至少要{2}個字", MinimumLength = 8)]
        public string buy_Address { get; set; }
    }
}