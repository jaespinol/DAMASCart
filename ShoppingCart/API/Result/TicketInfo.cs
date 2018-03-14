using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.API.Result
{
    public class Calificacion
    {
        public string IdCalificacion { get; set; }
        public string NoSerie { get; set; }
        public string Numero { get; set; }
        public string FacebookId { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Compartido { get; set; }
        public string Enviado { get; set; }
        public string IdUsuario { get; set; }
        public string IdProducto { get; set; }
        public string IdArticulo { get; set; }
        public string IdTicket { get; set; }
        public string Ciclo { get; set; }
        public string Comentario { get; set; }
        public string DNI { get; set; }
        public string Telefono { get; set; }

        public string Url
        {
            get
            {
                return $"{App.BaseUrl}/Calificar/{NoSerie}";
            }
        }

        public string QRCode
        {
            get
            {
                return $"{App.BaseUrl}/QRGenerator/Generate/?code={Url}";
            }
        }
    }

    public class Producto
    {
        public string nombre { get; set; }
        public string parte { get; set; }
        public string costo { get; set; }
        public string cantidad { get; set; }

        public string Descripcion
        {
            get
            {
                return $"Producto: {nombre} / {parte}";
            }
        }
        public string Ticket
        {
            get
            {
                return $"Cantidad {cantidad}, Costo total: {costo}";
            }
        }

        public string IdArticulo { get; set; }
        public string IdTicket { get; set; }
        public string IdProducto { get; set; }
        public string IdUsuario { get; set; }
        public List<Calificacion> Calificacion { get; set; }
    }

    public class TicketInfo
    {
        public int IdTicket { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public List<Producto> Articulos { get; set; }
    }
}
