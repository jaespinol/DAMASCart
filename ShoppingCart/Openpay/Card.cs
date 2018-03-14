using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Openpay
{
    public class Card
    {
        public string id { get; set; }
        public string type { get; set; }
        public string brand { get; set; }
        public Address address { get; set; }
        public string cvv2 { get; set; }
        public string device_session_id { get; set; }
        public string card_number { get; set; }
        public string holder_name { get; set; }
        public string expiration_year { get; set; }
        public string expiration_month { get; set; }
        public bool allows_charges { get; set; }
        public bool allows_payouts { get; set; }
        public DateTime creation_date { get; set; }
        public string bank_name { get; set; }
        public string customer_id { get; set; }
        public string bank_code { get; set; }

        public string CardNumber
        {
            get
            {
                if (card_number.Length > 12)
                {
                    var ultimosdigitos = card_number.Substring(12);
                    return "************" + ultimosdigitos;
                }
                else
                {
                    return card_number;
                }
            }
        }

        public string ExpirationDate
        {
            get
            {
                return $"{expiration_month}/{expiration_year}";
            }
        }
    }
}
