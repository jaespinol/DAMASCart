using Plugin.Connectivity;
using ShoppingCart.API.Result;
using ShoppingCart.Function;
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
    public partial class DirectionPage : ContentPage
    {
        public DirectionPage(Direction direccion)
        {
            InitializeComponent();
            BindingContext = direccion;
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                var provincia = BoxProvincia.Text ?? "";
                var ciudad = BoxCiudad.Text ?? "";
                var direccion = BoxDireccion.Text ?? "";
                var cp = BoxCP.Text ?? "";

                if (await this.TextValidate(App.AppName, "Aceptar",
                    new ValidateItem(provincia, "Ingresa un estado"),
                    new ValidateItem(ciudad, "Ingresa tu ciudad"),
                    new ValidateItem(direccion, "Ingresa tu direcicon"),
                    new ValidateItem(cp, "Ingresa tu codigo postal")))
                {
                    var idusuario = App.Oauth != null ? App.Oauth.IdUsuario.ToString() : "0";
                    DirectionResult result = await App.RestClient.Post<DirectionResult>(App.BaseUrl + "/Direction/Add", new Dictionary<string, object>
                    {
                        { "idusuario", idusuario },
                        { "direccion", direccion },
                        { "ciudad", ciudad },
                        { "codigopostal", cp },
                        { "provincia", provincia }
                    });

                    if (result != null)
                    {

                        await DisplayAlert(App.AppName, result.Message, "Aceptar");

                        DirectionResult direction = await App.RestClient.Get<DirectionResult>(App.BaseUrl + "/Direction/Get/" + App.Oauth.IdUsuario, new Dictionary<string, object> { });
                        if (direction != null)
                        {
                            if (direction.Direccion != null)
                            {
                                App.Oauth.Direction = direction.Direccion;
                            }
                        }

                        App.SetPreferences(App.Oauth);
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert(App.AppName, "No fue posible guardar los cambios, intenta mas tarde", "Aceptar");
                    }
                }
            }
            else
            {
                await DisplayAlert(App.AppName, "Debes de tener una conexion a internet activa", "Aceptar");
            }
        }
    }
}
