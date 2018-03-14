using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShoppingCart.Openpay
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DeviceSessionClient : ContentPage
	{
		public DeviceSessionClient ()
		{
			InitializeComponent ();
            OpenPayBackend.Source = $"{App.BaseUrl}/Payment/GetDeviceSessionId?idcliente={App.IdCliente}";
            OpenPayBackend.Navigated += OpenPayBackend_Navigated;
        }

        public event EventHandler<string> OnSuccess;

        private void OpenPayBackend_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var splitstring = e.Url.Split('/');
            if (splitstring.Length == 6)
            {
                var devicesessionid = splitstring[5];
                OnSuccess?.Invoke(this, devicesessionid);
            }
        }
    }
}