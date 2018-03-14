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
    public partial class AddGarantiaPage : ContentPage
    {
        public Calificacion _califcacion { get; set; }

        public AddGarantiaPage(Calificacion calificacion)
        {
            InitializeComponent();
            _califcacion = calificacion;
            BindingContext = _califcacion;

            Device.BeginInvokeOnMainThread(() =>
            {
                if (string.IsNullOrEmpty(_califcacion.Nombre))
                {
                    StackAdd.IsVisible = true;
                }
                else
                {
                    StackAdd.IsVisible = false;
                }
            });
        }

        private async void BtnContinuar_Clicked(object sender, EventArgs e)
        {
            if(await this.TextValidate(App.AppName, "Aceptar", 
                new ValidateItem(_califcacion.Nombre, "Ingresa un nombre"),
                new ValidateItem(_califcacion.Email, "Ingresa un correo", ValidationType.Email, "Ingresa un correo válido"),
                new ValidateItem(_califcacion.DNI, "Ingresa un DNI"),
                new ValidateItem(_califcacion.Telefono, "Ingresa un teléfono", ValidationType.Phone, "Ingresa un teléfono válido")))
            {
                // actualizamos el registro de calificación
                var result = await App.RestClient.Post<UpdateResult>($"{App.BaseUrl}/Review/update/{_califcacion.IdCalificacion}", new Dictionary<string, object>
                {
                    { "Nombre", _califcacion.Nombre },
                    { "Email", _califcacion.Email },
                    { "DNI", _califcacion.DNI },
                    { "Telefono", _califcacion.Telefono },
                });

                if (result != null)
                {
                    if (result.UpdateStatus)
                    {
                        await DisplayAlert(App.AppName, "Se han guardado los datos", "Aceptar");
                        await Navigation.PopAsync();
                        return;
                    }
                }

                await DisplayAlert(App.AppName, "Ocurrio un error al guardar los datos, intenta de nuevo más tarde", "Aceptar");
            }
        }
    }

}
