using DevAzt.FormsX.Net.HttpClient;
using Newtonsoft.Json;
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
	public partial class ProcessPaymentPage : ContentPage
	{
        private double _amount { get; set; }
        private string _description { get; set; }
        private string _customerid { get; set; }
        private string _baseurl { get; set; }

        public static ProcessPaymentPage Instance { get; set; }

        public ProcessPaymentPage (double amount, string description, string customerid, string baseurl)
		{
			InitializeComponent ();
            Instance = this;
            _amount = amount;
            _description = description;
            _customerid = customerid;
            _baseurl = baseurl;
            ProgressIndicator.IsVisible = true;
            OpenPayBackend.Navigated += OpenPayBackend_Navigated;
            BtnCancelBuy.Clicked += BtnCancelBuy_Clicked;
        }

        protected override bool OnBackButtonPressed()
        {
            CancelPayment?.Invoke(this, EventArgs.Empty);
            Instance = null;
            return base.OnBackButtonPressed();
        }

        public event EventHandler<EventArgs> CancelPayment;

        private void BtnCancelBuy_Clicked(object sender, EventArgs e)
        {
            CancelPayment?.Invoke(this, e);
            Instance = null;
        }

        protected override void OnDisappearing()
        {
            CancelPayment?.Invoke(this, EventArgs.Empty);
            Instance = null;
            base.OnDisappearing();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!string.IsNullOrEmpty(_customerid))
            {
                RestForms restclient = new RestForms();
                var listofcards = await restclient.Get<List<Card>>($"{_baseurl}/Payment/GetCustomerCards/{_customerid}/{App.IdCliente}", new Dictionary<string, object>());
                if(listofcards != null)
                {
                    ListOfCards.ItemsSource = listofcards;
                }
                else
                {
                    ErrorMessage.IsVisible = true;
                }
            }
            else
            {
                ErrorMessage.IsVisible = true;
            }

            ProgressIndicator.IsVisible = false;
        }

        private void ListOfCards_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListOfCards.SelectedItem = null;
        }

        Card _card;

        private void ListOfCards_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Card card)
            {
                _card = card;
                FormCvvCard.IsVisible = true;
                FormCvvCard.BindingContext = _card;
            }
        }

        private async void BtnContinue_Clicked(object sender, EventArgs e)
        {
            if(_card != null)
            {
                var cvv = BoxCvv.Text ?? "";
                _card.cvv2 = cvv;
                StackListCards.IsVisible = false;
                FormCvvCard.IsVisible = false;
                ProgressIndicator.IsVisible = true;
                var id = _card.id;
                var cardinsystem = await App.RestClient.Get<API.Result.Card>($"{App.BaseUrl}/Card/Get/{id}", new Dictionary<string, object>());
                if(cardinsystem != null)
                {
                    _card.card_number = cardinsystem.Number;
                    var cardjson = JsonConvert.SerializeObject(_card);
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(cardjson);
                    var base64 = System.Convert.ToBase64String(plainTextBytes);
                    var urltokenizercard = $"http://androidshopingcart.devaztio.com/Payment/Tokenizer?idcliente={App.IdCliente}&json={base64}";
                    System.Diagnostics.Debug.WriteLine(urltokenizercard, "Android Shoping Cart");
                    OpenPayBackend.Source = urltokenizercard;
                }
                else
                {
                    await DisplayAlert(App.AppName, "No podemos realizaar la transaccion, intenta mas tarde", "Aceptar");
                    StackListCards.IsVisible = true;
                    FormCvvCard.IsVisible = false;
                    ProgressIndicator.IsVisible = false;
                }
            }
        }

        private string _token { get; set; }
        private async void OpenPayBackend_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var containsparams = e.Url.Contains("?");
            if (containsparams) { }
            else
            {
                var items = e.Url.Split('/');
                if(items.Length == 6)
                {
                    _token = items[5];
                    DeviceSessionClient client = new DeviceSessionClient();
                    client.OnSuccess += Client_OnSuccess;
                    await Navigation.PushAsync(client);
                }
            }
        }

        public event EventHandler<ChargeResult> ChargeResult;

        private async void Client_OnSuccess(object sender, string e)
        {
            await Navigation.PopAsync();
            RestForms restclient = new RestForms();
            var response = await restclient.Post<ChargeResult, Charge>($"{_baseurl}/Payment/AddChargeToCard/{_customerid}/{App.IdCliente}", new Charge
            {
                amount = _amount,
                capture = false,
                currency = "MXN",
                cvv2 = _card.cvv2,
                description = _description,
                device_session_id = e,
                source_id = _token,
                order_id = $"{_token}_droidshop",
                method = "card"
            });

            if (response != null && response.status == Openpay.ChargeResult.completed)
            {
                ChargeResult?.Invoke(this, response);
                Instance = null;
            }
            else
            {
                await DisplayAlert("Pago con tarjeta", "Tuvimos un problema para realizar la transaccion con esta tarjeta, verifica el cvv, intenta de nuevo o intenta con otra tarjeta", "Aceptar");
                Device.BeginInvokeOnMainThread(() =>
                {
                    ProgressIndicator.IsVisible = false;
                    StackListCards.IsVisible = true;
                    FormCvvCard.IsVisible = false;
                });
            }
        }
    }
}