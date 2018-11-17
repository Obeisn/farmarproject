using System.ComponentModel.DataAnnotations;

namespace farmarproject.Models
{
    public class multi_buyMetadata
    {
        [Display(Name ="團購編號")]
        public int multi_buy_id { get; set; }
        [Display(Name ="開團名稱")]
        public string multi_buy_name { get; set; }
        [Display(Name ="發起人帳號")]
        public string raiser_id { get; set; }
        [Display(Name ="發起時間")]
        public System.DateTime raise_time { get; set; }
        [Display(Name ="截止時間")]
        public System.DateTime deadline { get; set; }
        [Display(Name ="人數上限")]
        public int mostpeople { get; set; }
    }
}