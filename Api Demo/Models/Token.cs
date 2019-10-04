using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDemo.Models
{
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public DateTime expire_time { get; set; }
    }
}