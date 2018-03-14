
using Plugin.Connectivity;
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
	public partial class AddCustomerPage : ContentPage
	{
		public AddCustomerPage ()
		{
			InitializeComponent ();
            BtnAceptar.Clicked += BtnAceptar_Clicked;
            StackuserInfo.BindingContext = App.Oauth;
            if(App.Oauth.Direction != null)
            {
                StackDirectionInfo.BindingContext = App.Oauth.Direction;
            }
		}

        private async void BtnAceptar_Clicked(object sender, EventArgs e)
        {
            // Si no hay un customer id
            if (string.IsNullOrEmpty(App.Oauth.CustomerId))
            {
                BtnAceptar.IsEnabled = false;
                if (CrossConnectivity.Current.IsConnected)
                {
                    var name = BoxName.Text ?? "";
                    var lastname = BoxLastName.Text ?? "";
                    var email = BoxEmail.Text ?? "";
                    int.TryParse(BoxPhone.Text ?? "0", out int phone);
                    var state = BoxState.Text ?? "";
                    var city = BoxCity.Text ?? "";
                    int.TryParse(BoxPostalCode.Text ?? "0", out int postalcode);
                    var line1 = BoxLine1.Text ?? "";
                    var line2 = BoxLine2.Text ?? "";
                    var line3 = BoxLine3.Text ?? "";

                    if (await this.TextValidate(App.AppName, "Aceptar",
                        new ValidateItem(name, "Ingresa un nombre"),
                        new ValidateItem(lastname, "Ingresa tus apellidos"),
                        new ValidateItem(email, "Ingresa un correo electronico"),
                        new ValidateItem(phone.ToString(), "Ingresa un telefono"),
                        new ValidateItem(state, "Ingresa el nombre del estado"),
                        new ValidateItem(city, "Ingresa tu ciudad"),
                        new ValidateItem(postalcode.ToString(), "Ingres tu codigo postal"),
                        new ValidateItem(line1, "Ingresa tu direccion"),
                        new ValidateItem(line2, "Ingresa otra direccion"),
                        new ValidateItem(line3, "Ingresa una referencia")))
                    {
                        Customer customer = await App.RestClient.Post<Customer, Customer>($"{App.BaseUrl}/Payment/AddClient/{App.IdCliente}", new Customer
                        {
                            name = name,
                            last_name = lastname,
                            email = email,
                            phone_number = phone,
                            external_id = App.Oauth.IdUsuario.ToString(),
                            requires_account = false,
                            address = new Address
                            {
                                city = city,
                                state = state,
                                postal_code = postalcode,
                                country_code = "MX",
                                line1 = line1,
                                line2 = line2,
                                line3 = line3
                            }
                        });

                        if (customer != null)
                        {
                            App.Oauth.CustomerId = customer.id;
                            App.SetPreferences(App.Oauth);
                            var result = await App.RestClient.Post<UpdateResult>($"{App.BaseUrl}/User/UpdateData/{App.Oauth.IdUsuario}", new Dictionary<string, object>
                            {
                                { "CustomerId", customer.id }
                            }) ?? new UpdateResult { UpdateStatus = false };

                            if (result.UpdateStatus)
                            {
                                await Navigation.PushAsync(new AddCardPage(customer));
                            }
                            else
                            {
                                await DisplayAlert(App.AppName, "No podemos guardar tus datos, intenta nuevamente", "Aceptar");
                            }
                        }
                        else
                        {
                            await DisplayAlert(App.AppName, "No podemos guardar tus datos, intenta nuevamente", "Aceptar");
                        }
                    }
                }
                BtnAceptar.IsEnabled = true;
            }
            else
            {
                // navegamos hacia la parte donde se agregan las tarjetas
                var customer = await App.RestClient.Post<Customer>($"{App.BaseUrl}/Payment/GetClient/{App.Oauth.CustomerId}/{App.IdCliente}", new Dictionary<string, object>());
                if(customer != null)
                {
                    await Navigation.PushAsync(new AddCardPage(customer));
                }
            }
        }

        private void BtnContinue_Clicked(object sender, EventArgs e)
        {

        }
    }
}