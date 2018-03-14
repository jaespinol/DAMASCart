using ShoppingCart.API;
using ShoppingCart.API.Result;
using DevAzt.FormsX.Storage.Isolated;
using ShoppingCart.ORM;
using Plugin.Connectivity;
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
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            BtnLogin.IsEnabled = false;
            var alias = BoxUser.Text ?? "";
            var password = BoxPassword.Text ?? "";

            if (CrossConnectivity.Current.IsConnected)
            {
                if (!string.IsNullOrEmpty(alias) && !string.IsNullOrEmpty(password))
                {
                    try
                    {
                        OauthResult result = await App.RestClient.Post<OauthResult>(App.BaseUrl + "/User/Oauth", new Dictionary<string, object>
                        {
                            { "alias", alias },
                            { "password", password }
                        });

                        if (result != null)
                        {
                            if (result.Code == 100)
                            {
                                DirectionResult direction = await App.RestClient.Get<DirectionResult>(App.BaseUrl + "/Direction/Get/" + result.IdUsuario, new Dictionary<string, object> { });
                                App.Oauth = result;
                                App.Oauth.Alias = alias;
                                App.Oauth.Password = password;
                                if (direction != null)
                                {
                                    if (direction.Direccion != null)
                                    {
                                        App.Oauth.Direction = direction.Direccion;
                                    }
                                }
                                App.SetPreferences(App.Oauth);
                                await GoToPage(result);
                            }
                            else
                            {
                                await DisplayAlert(App.AppName, result.Message, "Aceptar");
                            }
                        }
                        else
                        {
                            await DisplayAlert(App.AppName, "No es posible acceder, intenta más tarde", "Aceptar");
                        }
                    }
                    catch
                    {
                        await DisplayAlert(App.AppName, "No es posible acceder, intenta más tarde", "Aceptar");
                    }
                }
                else
                {
                    await DisplayAlert(App.AppName, "Introduce los datos que se te piden", "Aceptar");
                }
            }
            else
            {
                await DisplayAlert(App.AppName, "Asegurate de tener conexion a internet", "Aceptar");
            }
            BtnLogin.IsEnabled = true;
        }

        private async Task GoToPage(OauthResult result)
        {
            if (result.IdPerfil == 4) // usuario final
            {
                await Navigation.PushAsync(new ProductsPage() { Title = App.AppName });
            }
            else
            {
                // no hacemos nada
            }
        }
        
        private async void BtnSignin_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SIgninPage());
        }

        private async void BtnRecovery_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecoveryPassword());
        }
    }
}
