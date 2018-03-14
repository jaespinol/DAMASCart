using ShoppingCart.API.Result;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ShoppingCart.Openpay;

namespace ShoppingCart
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserProfilePage : TabbedPage
    {

        private OauthResult _oauth;
        ObservableCollection<Openpay.Card> _cards;

        public UserProfilePage(OauthResult oauth)
        {
            InitializeComponent();
            _oauth = oauth;
            StackUser.BindingContext = _oauth;
            if (_oauth.Direction != null)
            {
                StackDirection.BindingContext = _oauth.Direction;
                StackDirection.IsVisible = true;
                BtnDireccion.Text = "Actualizar dirección";
            }
            _tickets = new ObservableCollection<Ticket>();
            ListOfPurchases.ItemsSource = _tickets;
            _cards = new ObservableCollection<Openpay.Card>();
            ListOfCards.ItemsSource = _cards;
        }

        private async void BtnDireccion_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DirectionPage(_oauth.Direction));
        }

        private async void BtnAgregarTarjeta_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(App.Oauth.CustomerId))
            {
                await Navigation.PushAsync(new AddCustomerPage());
            }
            else
            {
                // navegamos hacia la parte donde se agregan las tarjetas
                var customer = await App.RestClient.Post<Customer>($"{App.BaseUrl}/Payment/GetClient/{App.Oauth.CustomerId}/{App.IdCliente}", new Dictionary<string, object>());
                if (customer != null)
                {
                    await Navigation.PushAsync(new AddCardPage(customer));
                }
            }
        }

        private async Task GetTickets()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                var result = await App.RestClient.Get<TicketsResult>(App.BaseUrl + "/Ticket/OfClient?idusuario=" + _oauth.IdUsuario, new Dictionary<string, object> { });
                if (result != null && result.Code == 100 && result.Tickets != null && result.Tickets.Count > 0)
                {
                    var tickets = result.Tickets;

                    if (tickets.Count > 0)
                    {
                        _tickets.Clear();
                        _tickets.Add(new Ticket
                        {
                            IdTicket = "#",
                            Fecha = "Fecha",
                            Paqueteria = "Código de paqueteria",
                            Total = "Total"
                        });
                        foreach (var ticket in tickets)
                        {
                            _tickets.Add(ticket);
                        }
                    }
                }
                else
                {

                }
            }
            else
            {
                await DisplayAlert(App.AppName, "Revisa tu conexión a internet", "Aceptar");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await GetTickets();

            if (App.Oauth.Direction != null)
            {
                StackDirection.BindingContext = App.Oauth.Direction;
                StackDirection.IsVisible = true;
                BtnDireccion.Text = "Actualizar dirección";
            }

            if (!string.IsNullOrEmpty(App.Oauth.CustomerId))
            {
                var listofcards = await App.RestClient.Get<List<Openpay.Card>>($"{App.BaseUrl}/Payment/GetCustomerCards/{App.Oauth.CustomerId}/{App.IdCliente}", new Dictionary<string, object>());
                if(listofcards != null)
                {
                    _cards.Clear();
                    foreach (var card in listofcards)
                    {
                        _cards.Add(card);
                    }
                }
            }
        }

        private void ListOfCards_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListOfCards.SelectedItem = null;
        }

        private async void ListOfCards_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if(e.Item is Openpay.Card card)
            {
                const string eliminar = "Eliminar";
                var result = await DisplayActionSheet(App.AppName, "Aceptar", null, eliminar);
                if (!string.IsNullOrEmpty(result))
                {
                    if(result == eliminar)
                    {
                        // eliminamos la tarjeta
                        var deleteresult = await App.RestClient.Post<StandarResult>($"{App.BaseUrl}/Payment/DeleteCustomerCard/{App.Oauth.CustomerId}/{card.id}/{App.IdCliente}", new Dictionary<string, object>());
                        if(deleteresult != null && deleteresult.Success)
                        {
                            _cards.Remove(card);
                        }
                    }
                }
            }
        }

        private bool inrefreshing = false;
        private ObservableCollection<Ticket> _tickets;

        private async void ListOfPurchases_Refreshing(object sender, EventArgs e)
        {
            if (!inrefreshing)
            {
                inrefreshing = true;
                await GetTickets();
                ListOfPurchases.EndRefresh();
                inrefreshing = false;
            }
        }

        private void ListOfPurchases_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListOfPurchases.SelectedItem = null;
        }

        private async void ListOfPurchases_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Ticket ticket)
            {
                if(ticket.IdTicket != "#") await Navigation.PushAsync(new InfoTicket(ticket));
            }
        }
    }
}