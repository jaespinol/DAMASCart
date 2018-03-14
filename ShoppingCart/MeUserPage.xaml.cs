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
	public partial class MeUserPage : ContentPage
	{
		public MeUserPage ()
		{
			InitializeComponent ();
            BindingContext = App.Oauth;
		}

        private async void BtnGuardar_Clicked(object sender, EventArgs e)
        {
            if (await this.TextValidate(App.AppName, "Aceptar",
                new ValidateItem(App.Oauth.Alias, "El alias no puede quedar vacío"),
                new ValidateItem(App.Oauth.Password, "La contraseña no puede quedar vacío"),
                new ValidateItem(App.Oauth.Correo, "El correo no puede quedar vacío", ValidationType.Email),
                new ValidateItem(App.Oauth.Nombre, "El nombre no puede quedar vacío"),
                new ValidateItem(App.Oauth.Telefono, "El teléfono no puede quedar vacío")))
            {
                if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
                {
                    var standarresult = await App.RestClient.Post<StandarResult>($"{App.BaseUrl}/User/ActualizarUsuario/{App.Oauth.IdUsuario}", new Dictionary<string, object>
                    {
                        { "Alias", App.Oauth.Alias },
                        { "Password", App.Oauth.Password },
                        { "Correo", App.Oauth.Correo },
                        { "Nombre", App.Oauth.Nombre },
                        { "Telefono", App.Oauth.Telefono }
                    });

                    if (standarresult != null)
                    {
                        if (standarresult.Success)
                        {
                            await DisplayAlert(App.AppName, "Se ha actualizado la información", "Aceptar");
                            App.SetPreferences(App.Oauth);
                        }
                        else
                        {
                            await DisplayAlert(App.AppName, "Intente de nuevo más tarde", "Aceptar");
                        }
                    }
                    else
                    {
                        await DisplayAlert(App.AppName, "Intente de nuevo más tarde", "Aceptar");
                    }
                }
                else
                {
                    await DisplayAlert(App.AppName, "No tienes conexión a internet", "Aceptar");
                }
            }
        }
    }
}