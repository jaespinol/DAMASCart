using ShoppingCart.API;
using ShoppingCart.API.Result;
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
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            if (App.Oauth != null)
            {
                BindingContext = App.Oauth;
                StackRedTecnicos.IsVisible = true;
                SwitchAcceptTerms.IsToggled = App.Oauth.RedTecnicos > 0;
            }
            Direction();
        }

        private async void Direction()
        {
            if (App.Oauth != null)
            {
                if (App.Oauth.HasDirection == 0 && App.Oauth.Direction == null)
                {
                    CreateDirection();
                }
                else
                {
                    if (App.Oauth.Direction == null)
                    {
                        CreateDirection();
                    }
                    else
                    {
                        SetDirection(App.Oauth.Direction);
                    }
                }
            }
            else
            {
                // CreateDirection();
                await DisplayAlert(App.AppName, "Debes iniciar sesión", "Aceptar");
                await Navigation.PopAsync();
            }
        }

        private void SetDirection(Direction direction)
        {
            if (direction == null) return;
            Device.BeginInvokeOnMainThread(() =>
            {
                RootLayout.Children.Clear();
                Label info = new Label { Text = "Estos son tus datos para tus envios..." };
                RootLayout.Children.Add(info);

                StackLayout formulario = new StackLayout();
                Label labeldnicuit = new Label { Text = "DNI: " + direction.Dnicuit };
                Label labelempresa = new Label { Text = "Enviar vía: " + direction.Empresa };
                Label labeldireccion = new Label { Text = "Dirección: " + direction.Direccion };
                Label labelciudad = new Label { Text = "Ciudad: " + direction.Ciudad };
                Label labelcp = new Label { Text = "Código postal: " + direction.CodigoPostal };
                Label labelprovincia = new Label { Text = "Provincia: " + direction.Provincia };

                formulario.Children.Add(labeldnicuit);
                formulario.Children.Add(labelempresa);
                formulario.Children.Add(labeldireccion);
                formulario.Children.Add(labelciudad);
                formulario.Children.Add(labelcp);
                formulario.Children.Add(labelprovincia);

                RootLayout.Children.Add(formulario);

                Button btnupdate = new Button { Text = "Actualizar información" };
                btnupdate.Clicked += Btnupdate_Clicked;
                RootLayout.Children.Add(btnupdate);

                if (!string.IsNullOrEmpty(App.Oauth.Nombre) || string.IsNullOrEmpty(App.Oauth.Alias))
                {
                    StackPage.IsVisible = true;
                }
            });
        }

        private void Btnupdate_Clicked(object sender, EventArgs e)
        {
            CreateDirection();
        }

        private void CreateDirection()
        {
            var currentdirection = new Direction();
            if(App.Oauth.Direction != null)
            {
                currentdirection = App.Oauth.Direction;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                RootLayout.Children.Clear();
                Label info = new Label { Text = "Tus pedidos necesitan una dirección" };
                RootLayout.Children.Add(info);
                StackLayout formulario = new StackLayout();
                Label labeldnicuit = new Label { Text = "Ingresa tu DNI o CUIT" };
                Entry boxdnicuit = new Entry { Placeholder = "DNI o CUIT", Text = currentdirection != null ? currentdirection.Dnicuit : "" };
                Label labelempresa = new Label { Text = "Nombre de la empresa con la que recibes tu paquetería." };
                Entry boxempresa = new Entry { Placeholder = "DHL/FedEx", Text = currentdirection != null ? currentdirection.Empresa : "" };
                Label labeldireccion = new Label { Text = "Ingresa tu dirección" };
                Entry boxdireccion = new Entry { Placeholder = "Calle, No. Interior, No. Exterior", Text = currentdirection != null ? currentdirection.Direccion : "" };
                Label labelciudad = new Label { Text = "Ingresa el nombre de tu ciudad" };
                Entry boxciudad = new Entry { Placeholder = "Ciudad", Text = currentdirection != null ? currentdirection.Ciudad : "" };
                Label labelcp = new Label { Text = "Código Postal" };
                Entry boxcp = new Entry { Placeholder = "274883", Text = currentdirection != null ? currentdirection.CodigoPostal : "" };
                Label labelprovincia = new Label { Text = "Ingresa el nombre de tu provincia" };
                Entry boxprovincia = new Entry { Placeholder = "Provincia", Text = currentdirection != null ? currentdirection.Provincia : "" };
                Button btnsave = new Button { Text = "Guardar" };
                btnsave.Clicked += async (s, e) =>
                {
                    var idusuario = App.Oauth != null ? App.Oauth.IdUsuario.ToString() : "0";
                    DirectionResult result = await App.RestClient.Post<DirectionResult>(App.BaseUrl + "/Direction/Add", new Dictionary<string, object>
                    {
                        { "idusuario", idusuario },
                        { "dnicuit", boxdnicuit.Text ?? "" },
                        { "empresa", boxempresa.Text ?? "" },
                        { "direccion", boxdireccion.Text ?? "" },
                        { "ciudad", boxciudad.Text ?? "" },
                        { "codigopostal", boxcp.Text ?? "" },
                        { "provincia", boxprovincia.Text ?? "" }
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
                                SetDirection(direction.Direccion);
                            }
                        }
                    }
                };
                formulario.Children.Add(labeldnicuit);
                formulario.Children.Add(boxdnicuit);
                formulario.Children.Add(labelempresa);
                formulario.Children.Add(boxempresa);
                formulario.Children.Add(labeldireccion);
                formulario.Children.Add(boxdireccion);
                formulario.Children.Add(labelciudad);
                formulario.Children.Add(boxciudad);
                formulario.Children.Add(labelcp);
                formulario.Children.Add(boxcp);
                formulario.Children.Add(labelprovincia);
                formulario.Children.Add(boxprovincia);
                formulario.Children.Add(btnsave);
                RootLayout.Children.Add(formulario);

            });
        }

        private void MyProfilePage_Clicked(object sender, EventArgs e)
        {
            if (App.Oauth != null)
            {
                if (!string.IsNullOrEmpty(App.Oauth.Alias))
                {
                    Device.OpenUri(new Uri($"{App.BaseUrl}/Tecnico/{App.Oauth.Alias}"));
                }
                else if(!string.IsNullOrEmpty(App.Oauth.Nombre))
                {
                    Device.OpenUri(new Uri($"{App.BaseUrl}/Tecnico/{App.Oauth.Nombre}"));
                }
                else
                {

                }
            }
            
        }

        private async void SwitchAcceptTerms_Toggled(object sender, ToggledEventArgs e)
        {
            SwitchAcceptTerms.Toggled -= SwitchAcceptTerms_Toggled;

            if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
            {
                var standarresult = await App.RestClient.Post<StandarResult>($"{App.BaseUrl}/User/ActualizarUsuario/{App.Oauth.IdUsuario}", new Dictionary<string, object>
                {
                    { "RedTecnicos", e.Value ? 1 : 0 }
                });

                if (standarresult != null)
                {
                    if (standarresult.Success)
                    {
                        SwitchAcceptTerms.IsToggled = e.Value;
                        App.Oauth.RedTecnicos = e.Value ? 1 : 0;
                    }
                    else
                    {
                        SwitchAcceptTerms.IsToggled = !e.Value;
                        await DisplayAlert(App.AppName, "Intente de nuevo más tarde", "Aceptar");
                    }
                }
                else
                {
                    SwitchAcceptTerms.IsToggled = !e.Value;
                    await DisplayAlert(App.AppName, "Intente de nuevo más tarde", "Aceptar");
                }
            }
            else
            {
                SwitchAcceptTerms.IsToggled = !e.Value;
                await DisplayAlert(App.AppName, "No tienes conexión a internet", "Aceptar");
            }
            
            SwitchAcceptTerms.Toggled += SwitchAcceptTerms_Toggled;
        }

        private void BtnLocalGPS_Clicked(object sender, EventArgs e)
        {
            LocationTask task = new LocationTask(this);
            //task.LocationComplete += Task_LocationComplete;
            // task.Show();
        }

        private async void Task_LocationComplete(object sender, LocationComplete e)
        {
            if (e.Success)
            {
                if (App.Oauth != null)
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        // position as string
                        var position = e.Position.ToString();
                        // Update direction
                        var result = await App.RestClient.Post<UpdateResult>($"{App.BaseUrl}/User/UpdateGPS/{App.Oauth.IdUsuario}", new Dictionary<string, object>
                        {
                            { "GPS", position },
                            { "Local", "1" },
                            { "SearchText", e.SearchText }
                        });

                        if (result != null)
                        {
                            if (result.UpdateStatus)
                            {
                                await DisplayAlert(App.AppName, "¡¡Ahora tus clientes, saben donde te ubicas!!", "Aceptar");
                            }
                            else
                            {
                                await DisplayAlert(App.AppName, "Intenta más tarde... Código de error: HX00-0F00", "Aceptar");
                            }
                        }
                        else
                        {
                            await DisplayAlert(App.AppName, "Intenta más tarde... Código de error: HX00-0F01", "Aceptar");
                        }
                    }
                    else
                    {
                        await DisplayAlert(App.AppName, "Necesitas una conexión a internet", "Aceptar");
                    }
                }
                else
                {
                    await DisplayAlert(App.AppName, "Intenta más tarde... Código de error: HX00-0F02", "Aceptar");
                }
            }
            else
            {
                await DisplayAlert(App.AppName, "No se ingreso la dirección del local", "Aceptar");
            }
        }

        private async void BtnUserUpdate_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MePage());
        }
    }
}
