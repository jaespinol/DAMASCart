using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart
{ 

    public class Precio
    {
        public string IdPrecio { get; set; }
        public string Fecha { get; set; }
        public string PrecioUnitario { get; set; }
        public string PrecioMayoreo { get; set; }
        public string CantidadMayoreo { get; set; }
        public string CantidadUnitario { get; set; }
        public string IdProducto { get; set; }
        public string IdUsuario { get; set; }
    }

    public class Stock
    {
        public string IdProducto { get; set; }
        public string TotalStock { get; set; }
        public string Nombre { get; internal set; }
        public string Qty { get; internal set; }
        public string IdUsuario { get; internal set; }
    }

    public class Producto
    {
        public string IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Parte { get; set; }
        public string Fecha { get; set; }
        public string Color { get; set; }
        public string Calidad { get; set; }
        public string Tags { get; set; }
        public string Image { get; set; }
        public Precio Precio { get; set; }
        public Stock Stock { get; set; }
        public int Vigencia { get; set; }
        public int IdModelo { get; set; }
        public int IdCategoria { get; set; }
        public int? Minimo { get; set; }
        public string Operacion { get; set; }

        public string ImageUrl
        {
            get
            {
                return string.Format("{0}/uploads/{1}", App.BaseUrl, Image);
            }
        }
    }

    public class SearchResult
    {
        public string Tag { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public List<Producto> Productos { get; set; }
    }
}
