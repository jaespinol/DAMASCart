using System.Collections.Generic;

namespace ShoppingCart
{
    internal class StockRequest
    {
        public List<Stock> StockList { get;  set; }
        public string IdUsuario { get; set; }
    }
}