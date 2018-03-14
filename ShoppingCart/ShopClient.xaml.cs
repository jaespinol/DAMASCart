using ShoppingCart.API.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShoppingCart
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShopClient : ContentPage
    {
        public ShopClient(Ticket ticket)
        {
            InitializeComponent();
            Set(ticket);
        }

        private void Set(Ticket ticket)
        {
            TicketFecha.Text = "Fecha del pedido: " + ticket.Fecha;

            if (ticket.Usuario != null)
            {
                var user = ticket.Usuario;
                ClientName.Text = "Nombre del cliente: " + user.Nombre + " " + user.Apellido;
                ClientEmail.Text = "Correo del cliente: " + user.Correo;
                ClientPhone.Text = "Teléfono del cliente: " + user.Telefono;
            }

            if (ticket.Direccion != null)
            {
                var direction = ticket.Direccion;
                LocationDirection.Text = $"Direccion: {direction.Direccion}";
                LocationCity.Text = $"Ciudad: {direction.Ciudad}";
                LocationCP.Text = $"Código postal: {direction.CodigoPostal}";
                LocationOther.Text = $"Provincia: {direction.Provincia}";
            }
        }
    }
}
