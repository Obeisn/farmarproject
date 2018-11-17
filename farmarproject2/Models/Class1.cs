using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace farmarproject2.Models
{
    [MetadataType(typeof(UserMetaData))]
    public partial class AspNetUser
    {
       
        public class UserMetaData
        {
            [Display(Name = "姓名")]
            public string FamName { get; set; }
            [Display(Name = "照片")]
            public string UserIg { get; set; }
            [Display(Name = "信箱")]
            public string Email { get; set; }
            [Display(Name = "電話")]
            public string PhoneNumber { get; set; }
        }
    }
}