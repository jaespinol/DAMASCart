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
    public partial class InfoTicket : TabbedPage
    {
        public InfoTicket(Ticket ticket)
        {
            InitializeComponent();
            Set(ticket);
        }

        private async void Set(Ticket ticket)
        {
            var result = await App.RestClient.Post<TicketInfo>(App.BaseUrl + "/Ticket/Info", new Dictionary<string, object>
            {
                { "IdTicket", ticket.IdTicket.ToString() }
            });

            if (result != null)
            {
                Children.Add(new ShopClient(ticket) { Icon = "perfil.png" });
                Children.Add(new ShopList(result.Articulos) { Icon = "pedidos.png" });
                Children.Add(new ContactPage() { Icon = "infobancaria.png" });
            }
        }
    }
}
