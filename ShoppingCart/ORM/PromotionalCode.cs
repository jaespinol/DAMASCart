using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.ORM
{
    public class PromotionalCode
    {

        public int IdCodigo { get; set; }
        public string Nombre { get; set; }
        public string Referencia { get; set; }
        public string Descuento { get; set; }
        public string Fecha { get; set; }

    }
}
