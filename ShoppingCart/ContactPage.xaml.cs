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
    public partial class ContactPage : ContentPage
    {
        public ContactPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var contact = await App.RestClient.Get<Contact>($"{App.BaseUrl}/Contact/Get/{App.IdCliente}", new Dictionary<string, object>());
            if(contact != null)
            {
                LabelEmail.Text = $"Email: {contact.Email}";
                LabelTelefono.Text = $"WhatsApp: {contact.Telefono}";
                LabelDireccion.Text = $"Direccion: {contact.Direccion}";
            }
        }
    }
}
