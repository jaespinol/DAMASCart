using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Openpay
{
    public class Charge
    {
        public string method { get; set; }
        public string source_id { get; set; }
        public double amount { get; set; }
        public string cvv2 { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public string order_id { get; set; }
        public bool capture { get; set; }
        public string device_session_id { get; set; }
    }

}
