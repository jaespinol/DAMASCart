using DevAzt.FormsX.Device.Sensors;
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
	public partial class LocationTask : ContentPage
	{
        private bool _search { get; set; }
        private Page _page { get; set; }
        private Xamarin.Forms.Maps.Pin _pin { get; set; }
        private GeoPosition _geoposition { get; set; }

        public LocationTask(Page page)
        {
            InitializeComponent();
            _page = page;
        }
        
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            _geoposition = GeoPosition.Instance;
            if (await _geoposition.PermitsGranted())
            {
                if (_geoposition.StartGPS())
                {
                    _geoposition.PositionChange += Geoposition_PositionChange;
                }
            }
        }

        public int count = 0;
        public bool _finishposition = true;
        private void Geoposition_PositionChange(object sender, Position e)
        {
            if (e.Success)
            {
                if (_finishposition)
                {
                    _finishposition = false;
                    if (!_search)
                    {
                        GridMap.Children.Clear();
                        var position = new Xamarin.Forms.Maps.Position(e.Latitude, e.Longitude);
                        var map = new Xamarin.Forms.Maps.Map(Xamarin.Forms.Maps.MapSpan.FromCenterAndRadius(position, Xamarin.Forms.Maps.Distance.FromMiles(0.7)))
                        {
                            IsShowingUser = true,
                            VerticalOptions = LayoutOptions.FillAndExpand
                        };
                        map.Pins.Clear();
                        var pin = Pin(position, "Tu posición actual");
                        map.Pins.Add(pin);
                        GridMap.Children.Add(map);
                        _finishposition = false;
                    }
                    else
                    {
                        _finishposition = true;
                    }
                }
            }
        }

        private Xamarin.Forms.Maps.Pin Pin(Xamarin.Forms.Maps.Position position, string text = "")
        {
            _pin = new Xamarin.Forms.Maps.Pin
            {
                Position = position,
                Type = Xamarin.Forms.Maps.PinType.Generic,
                Label = text
            };
            return _pin;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (_pin == null)
            {
                OnLocationComplete(new LocationComplete
                {
                    Success = false
                });
            }
            else
            {
                OnLocationComplete(new LocationComplete
                {
                    SearchText = SearchText,
                    Position = new Position { Latitude = _pin.Position.Latitude, Longitude = _pin.Position.Longitude },
                    Success = true
                });
            }

            await Navigation.PopAsync();
        }
        

        public event EventHandler<LocationComplete> LocationComplete;

        private void OnLocationComplete(LocationComplete args)
        {
            LocationComplete?.Invoke(this, args);
        }

        public async void Show()
        {
            await _page.Navigation.PushAsync(this);
        }

        public string SearchText { get; set; }
        private async void BoxSearch_SearchButtonPressed(object sender, EventArgs e)
        {
            SearchText = BoxSearch.Text ?? "";
            
            if (await this.TextValidate(App.AppName, "Aceptar",
                    new ValidateItem(SearchText, "Ingresa una dirección")))
            {
                var geoCoder = new Xamarin.Forms.Maps.Geocoder();
                var approximateLocations = await geoCoder.GetPositionsForAddressAsync(SearchText);
                if (approximateLocations.Count() > 0)
                {
                    _search = true;
                    GridMap.Children.Clear();
                    var location = approximateLocations.ElementAt(0);
                    var map = new Xamarin.Forms.Maps.Map(Xamarin.Forms.Maps.MapSpan.FromCenterAndRadius(location, Xamarin.Forms.Maps.Distance.FromMiles(0.7)))
                    {
                        IsShowingUser = true,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };
                    map.Pins.Clear();
                    var pin = Pin(location, SearchText);
                    map.Pins.Add(pin);
                    GridMap.Children.Add(map);
                }
            }
        }
    }

    public class LocationComplete
    {
        public Position Position { get; set; }
        public bool Success { get; set; }
        public string SearchText { get; set; }
    }
}