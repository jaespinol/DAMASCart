using ShoppingCart.API;
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
    public partial class RecoveryPassword : ContentPage
    {
        public RecoveryPassword()
        {
            InitializeComponent();
        }

        private async void BtnRecovery_Clicked(object sender, EventArgs e)
        {
            var email = BoxEmail.Text ?? "";

            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert(App.AppName, "Ingresa un correo electronico", "Aceptar");
            }
            else
            { 
                StandarResult result = await App.RestClient.Post<StandarResult>($"{App.BaseUrl}/User/Recovery", new Dictionary<string, object>
                {
                    { "email", email }
                });
                if (result != null)
                {
                    await DisplayAlert(App.AppName, result.Message, "Aceptar");
                }
                else
                {
                    await DisplayAlert(App.AppName, $"No se encontro el correo...{email}", "Aceptar");
                }
            }
        }
    }
}
