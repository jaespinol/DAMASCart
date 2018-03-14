using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.API.Result
{
    public class StandarResult
    {

        public string Message { get; set; }
        public int Code { get; set; }
        public int InsertId { get; set; }
        public bool Success { get; set; }

    }
}
