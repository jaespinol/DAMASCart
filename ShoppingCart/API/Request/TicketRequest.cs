using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.API.Request
{
    public class TicketRequest
    {
        public int IdUsuario { get; set; }
        public int IdCliente { get; set; }
        public List<Articulo> Articulos { get; set; }
        public int IdCodigo { get; set; }
        public double Total { get; set; }

        public TicketRequest()
        {
            Articulos = new List<Articulo>();
        }
    }

    public class Articulo
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public double Costo { get; set; }
    }
}