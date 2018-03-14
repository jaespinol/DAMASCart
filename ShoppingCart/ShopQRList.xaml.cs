using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingCart.API.Result;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShoppingCart
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShopItemDetail : ContentPage
    {
        private Calificacion _calificacion;

        public ShopItemDetail(Calificacion calificacion)
        {
            InitializeComponent();
            _calificacion = calificacion;
            BindingContext = _calificacion;   
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            Device.BeginInvokeOnMainThread(() =>
            {
                if (string.IsNullOrEmpty(_calificacion.Nombre))
                {
                    StackInfo.BackgroundColor = Color.Transparent;
                    StackInfoText.Text = "Código QR para calificar esta reparación";
                }
                else
                {
                    StackInfo.BackgroundColor = Color.Green;
                    StackInfoText.Text = "Ya se ha usado QR...";
                    StackInfoText.TextColor = Color.White;
                }
            });
        }

        private async void BtnGarantia_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddGarantiaPage(_calificacion));
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(_calificacion.Url));
        }
    }
}
