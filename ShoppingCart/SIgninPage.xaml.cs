using DevAzt.FormsX.Device.SIM;
using ShoppingCart.API;
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
    public partial class SIgninPage : ContentPage
    {
        public SIgninPage()
        {
            InitializeComponent();
            string linenumber = "";
            try { linenumber = "+" + PhoneManager.Instance.LineNumber; } catch{ }
            if (!linenumber.Equals("+"))
            {
                BoxTelefono.Text = linenumber;
            }
        }

        private async void Registrarse_Clicked(object sender, EventArgs e)
        {
            var alias = BoxUsuario.Text ?? "";
            var password = BoxPassword.Text ?? "";
            var nombre = BoxNombre.Text ?? "";
            var apellido = BoxApellido.Text ?? "";
            var email = BoxEmail.Text ?? "";
            var telefono = BoxTelefono.Text ?? "";
            var emailverify = BoxEmailVerify.Text ?? "";

            if (await this.TextValidate(App.AppName, "Aceptar",
                new ValidateItem(nombre, "Ingresa tu nombre"),
                new ValidateItem(apellido, "Ingresa tus apellidos"),
                new ValidateItem(email, "Ingresa tu correo", ValidationType.Email, "Ingresa un correo válido"),
                new ValidateItem(emailverify, "Vuelve a ingresa tu correo", ValidationType.Email, "Vuelve a ingresa un correo válido"),
                new ValidateItem(telefono, "Ingresa tu teléfono"),
                new ValidateItem(alias, "Ingresa tu nickname"),
                new ValidateItem(password, "Ingresa tu contraseña"))
               )
            {
                if (email.Equals(emailverify))
                {
                    SigninResult result = await App.RestClient.Post<SigninResult>(App.BaseUrl + "/User/Add", simpleparams: new Dictionary<string, object>()
                    {
                        { "alias", alias },
                        { "password", password },
                        { "nombre", nombre },
                        { "apellido", apellido },
                        { "correo", email },
                        { "telefono", telefono },
                        { "idcliente", App.IdCliente }
                    });

                    if (result != null)
                    {
                        if (result.Message.Contains("Usuario agregado correctamente"))
                        {
                            await DisplayAlert(App.AppName, "Te has registrado con exito; te hemos enviado un correo con un enlace de activacion.", "Aceptar");
                            App.Instance.SetMainPage(new LoginPage());
                        }
                        else
                        {
                            await DisplayAlert(App.AppName, "Intenta con otro usuario u otro correo...", "Aceptar");
                        }
                    }
                    else
                    {
                        await DisplayAlert(App.AppName, "Intenta con otro usuario u otro correo...", "Aceptar");
                    }
                }
                else
                {
                    await DisplayAlert(App.AppName, "Los correos no son iguales", "Aceptar");
                }
            }
        }
    }
}
