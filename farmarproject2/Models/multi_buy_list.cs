//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace farmarproject2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class multi_buy_list
    {
        public int multi_buy_list_id { get; set; }
        public int multi_buy_id { get; set; }
        public string join_id { get; set; }
        public int amount { get; set; }
        public System.DateTime deadine { get; set; }
    
        public virtual multi_buy multi_buy { get; set; }
    }
}
