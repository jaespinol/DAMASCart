
using ShoppingCart.API.Result;
using ShoppingCart.Function;
using ShoppingCart.Openpay;
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
	public partial class AddCardPage : ContentPage
	{
        private Openpay.Card _card;

        private Customer _customer { get; set; }

		public AddCardPage (Customer customer)
		{
			InitializeComponent ();
            _customer = customer;
            BindingContext = customer;
            if(App.Oauth.Direction != null) StackDirectionInfo.BindingContext = App.Oauth.Direction;
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            BtnSave.IsEnabled = false;
            var number = BoxCardNumber.Text ?? "";
            var ccv = BoxCCV.Text ?? "";
            var city = BoxCity.Text ?? "";
            var holdername = BoxHolderName.Text ?? "";
            var line1 = BoxLine1.Text ?? "";
            var line2 =  BoxLine2.Text ?? "";
            var line3 = BoxLine3.Text ?? "";
            var month = BoxMonth.Text ?? "";
            int.TryParse(BoxPostalCode.Text ?? "0", out int cp);
            var state = BoxState.Text ?? "";
            var year = BoxYear.Text ?? "";

            if(await this.TextValidate(App.AppName, "Aceptar" ,
                new ValidateItem(holdername, "Ingresa el nombre del titular"),
                new ValidateItem(number, "Ingresa el numero de tarjeta"),
                new ValidateItem(ccv, "Ingresa el numero de cvv"),
                new ValidateItem(month, "Ingresa el mes de la tarjeta"),
                new ValidateItem(year, "Ingresa el año de la tarjeta"),
                new ValidateItem(state, "Ingresa el nombre del estado donde vives"),
                new ValidateItem(city, "Ingresa el nombre de tu ciudad"),
                new ValidateItem(cp.ToString(), "Ingresa tu codigo postal"),
                new ValidateItem(line1, "Ingresa tu direccion"),
                new ValidateItem(line2, "Ingresa otra direccion"),
                new ValidateItem(line3, "Ingresa una referencia")))
            {
                _card = new Openpay.Card
                {
                    address = new Address
                    {
                        city = city,
                        country_code = "MX",
                        line1 = line1,
                        line2 = line2,
                        line3 = line3,
                        postal_code = cp,
                        state = state
                    },
                    card_number = number,
                    cvv2 = ccv,
                    expiration_month = month,
                    expiration_year = year,
                    holder_name = holdername
                };

                DeviceSessionClient client = new DeviceSessionClient();
                client.OnSuccess += Client_OnSuccess;
                await Navigation.PushAsync(client);
            }
            else
            {
                BtnSave.IsEnabled = true;
            }
        }

        private async void Client_OnSuccess(object sender, string e)
        {
            await Navigation.PopAsync();
            if(_card != null)
            {
                _card.device_session_id = e;
                var result = await App.RestClient.Post<Openpay.Card, Openpay.Card>($"{App.BaseUrl}/Payment/AddCard/{App.Oauth.CustomerId}/{App.IdCliente}", _card);
                if(result != null)
                {

                    var addcardtosystem = await App.RestClient.Post<StandarResult>($"{App.BaseUrl}/Card/Add", new Dictionary<string, object>
                    {
                        { "number", _card.card_number },
                        { "openpayid", result.id }
                    });

                    if(addcardtosystem != null)
                    {
                        if (addcardtosystem.Success)
                        {

                        }
                        else
                        {
                            // guardamos en la base local para subir mas tarde
                        }
                    }
                    else
                    {
                        // guardamos en la base local para subir mas tarde
                    }

                    await DisplayAlert(App.AppName, "Se ha agregado tu tarjeta", "Aceptar");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert(App.AppName, "Tuvismo un problema al validar tu tarjeta, revisa los datos e intenta de nuevo.", "Aceptar");
                }
            }
            BtnSave.IsEnabled = true;
        }
    }
}