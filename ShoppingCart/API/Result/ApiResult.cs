using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.API.Result
{
    public class ApiResult<T>
    {

        public List<T> Result { get; set; }

    }
}
