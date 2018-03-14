using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShoppingCart.ViewModel
{
    public class AddToCar
    {
        public Entry BoxProducto { get; set; }
        public Command BtnProducto { get; set; }
        public Producto Product { get; internal set; }
    }
}
