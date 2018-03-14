using ShoppingCart.API;
using ShoppingCart.API.Result;
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
    public partial class FilterPage : ContentPage
    {
        public FilterPage()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            PickerFilter.Items.Add("Fecha");
            PickerFilter.Items.Add("Cliente");
            PickerFilter.Items.Add("Orden");
            PickerFilter.SelectedIndexChanged += PickerFilter_SelectedIndexChanged;
            if (PickerFilter.Items.Count > 0)
            {
                PickerFilter.SelectedIndex = 0;
            }
        }

        private void PickerFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterInputLayout.Children.Clear();
            var picker = sender as Picker;
            FilterModel model = new FilterModel();
            if (picker.SelectedIndex == 0)
            {
                DatePicker datepicker = new DatePicker { Date = DateTime.Now };
                FilterInputLayout.Children.Add(datepicker);
                model.TypeFilter = TypeFilter.Date;
                model.FilterInput = datepicker;
            }
            else if (picker.SelectedIndex == 1)
            {
                Entry entry = new Entry { Placeholder = "Nombre de algún cliente" };
                FilterInputLayout.Children.Add(entry);
                model.TypeFilter = TypeFilter.Client;
                model.FilterInput = entry;
            }
            else if (picker.SelectedIndex == 2)
            {
                Entry entry = new Entry { Placeholder = "26781", Keyboard = Keyboard.Numeric };
                FilterInputLayout.Children.Add(entry);
                model.TypeFilter = TypeFilter.Order;
                model.FilterInput = entry;
            }
            Button btnsearch = new Button { Text = "Buscar", Command = new Command<FilterModel>(SearchByFilter), CommandParameter = model };
            FilterInputLayout.Children.Add(btnsearch);
        }

        private async void SearchByFilter(FilterModel model)
        {
            try
            {
                var request = await model.MakeQuery();
                if (request != null)
                {
                    if (request.Code == 100)
                    {
                        await Navigation.PushAsync(new TicketPage(App.Oauth, tickets: request.Tickets));
                        return;
                    }
                }
                await DisplayAlert(App.AppName, "No encontramos ningun resultado", "Aceptar");
            }
            catch (Exception ex)
            {
                await DisplayAlert(App.AppName, ex.Message, "Aceptar");
            }
        }
    }

    public class FilterModel
    {
        public View FilterInput { get; set; }
        public TypeFilter TypeFilter { get; set; }

        public async Task<TicketsResult> MakeQuery()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            switch (TypeFilter)
            {
                case TypeFilter.Date:
                    var datepicker = FilterInput as DatePicker;
                    data.Add("fecha", datepicker.Date.ToString("yyyy-MM-dd"));
                    break;
                case TypeFilter.Client:
                    var clientbox = FilterInput as Entry;
                    data.Add("cliente", clientbox.Text);
                    break;

                case TypeFilter.Order:
                    var orderbox = FilterInput as Entry;
                    int idticket = 0;
                    if (int.TryParse(orderbox.Text, out idticket))
                    {
                        data.Add("ticket", orderbox.Text);
                    }
                    else
                    {
                        throw new Exception("Ingresa un número para la orden");
                    }
                    break;
            }
            
            return await App.RestClient.Get<TicketsResult>(App.BaseUrl + "/Ticket/Search", data);
        }
    }

    public enum TypeFilter
    {
        Date, Client, Order
    }
}
