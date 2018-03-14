using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShoppingCart.API.Result
{
    public class TicketsResult
    {
        public string Message { get; set; }
        public int Code { get; set; }
        public List<Ticket> Tickets { get; set; }
    }

    public class Ticket
    {
        public string IdTicket { get; set; }
        public string Fecha { get; set; }
        public string Baucher { get; set; }
        public string Paqueteria { get; set; }
        public string CodigoPaqueteria { get; set; }
        public string IdUsuario { get; set; }
        public string Total { get; set; }
        public string Verificado { get; set; }
        public Label Pedido { get; set; }
        public Label Estatus { get; set; }
        public Label Monto { get; set; }
        public Image BtnCamara { get; set; }
        public Direction Direccion { get; set; }
        public OauthResult Usuario { get; set; }
        public Image BtnLike { get; set; }
        public Image BtnDireccion { get; set; }
        public string Message { get; set; }
        public Command Info { get; set; }
    }
}
