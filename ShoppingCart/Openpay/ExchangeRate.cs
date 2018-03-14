using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Openpay
{
    public class ExchangeRate
    {
        public string from { get; set; }
        public string date { get; set; }
        public double value { get; set; }
        public string to { get; set; }
    }
}
