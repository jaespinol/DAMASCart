using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Openpay
{
    public class Address
    {
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string line3 { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public int postal_code { get; set; }
        public string country_code { get; set; }
    }
}
