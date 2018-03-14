using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Openpay
{
    public class ChargeResult
    {

        public const string completed = "completed";

        public string id { get; set; }
        public double amount { get; set; }
        public string authorization { get; set; }
        public string method { get; set; }
        public string operation_type { get; set; }
        public string transaction_type { get; set; }
        public Card card { get; set; }
        public string status { get; set; }
        public string currency { get; set; }
        public ExchangeRate exchange_rate { get; set; }
        public DateTime creation_date { get; set; }
        public DateTime operation_date { get; set; }
        public string description { get; set; }
        public object error_message { get; set; }
        public string order_id { get; set; }
    }
}
