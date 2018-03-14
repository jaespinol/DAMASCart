using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Openpay
{
    public class Customer
    {
        public string id { get; set; }
        public string external_id { get; set; }
        public string name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public bool requires_account { get; set; }
        public int phone_number { get; set; }
        public Address address { get; set; }
        public string clabe { get; set; }
        public string full_name
        {
            get
            {
                return $"{name} {last_name}";
            }
        }
    }
}
