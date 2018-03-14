using DevAzt.FormsX.Device.Sensors;
using DevAzt.FormsX.Storage.SQLite.LiteConnection;
using ShoppingCart.API;
using ShoppingCart.API.Result;
using DevAzt.FormsX.Storage.Isolated;
using ShoppingCart.ViewModel;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using DevAzt.FormsX.Net.HttpClient;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace ShoppingCart
{
    public partial class App : Application
    {

        public const string MerchantId = "md1uoqoa8u60bcmosktu";
        public const string ApiKey = "pk_015440e377d84dae92f4e38239a583f6";

        public static CultureInfo Culture
        {
            get
            {
                return new CultureInfo("es-MX");
            }
        }
        public static List<AddToCar> Products { get; set; }
        public static List<ShopItem> ShopList { get; set; }
        public static string BaseUrl
        {
            get
            {
                return "http://androidshopingcart.devaztio.com";
            }
        }
        public static string AppName
        {
            get
            {
                return "My POS";
            }
        }

        public static OauthResult Oauth { get; set; }

        public static App Instance { get; set; }
        public static RestForms RestClient { get; set; }
        public static GeoPosition GPS { get; private set; }
        public static int IdCliente = 2;

        public App()
        {
            InitializeComponent();
            var navigationpage = new NavigationPage(new BlankPage())
            {
                BarBackgroundColor = Color.FromHex("#8ea226"),
                BarTextColor = Color.White
            };
            MainPage = navigationpage;
        }

        private async void CheckPermissions()
        {
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                bool success = false;
                Dictionary<Permission, PermissionStatus> permissions = new Dictionary<Permission, PermissionStatus>();
                try
                {
                    permissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location, Permission.Phone, Permission.Storage);
                    success = true;
                }
                catch (Exception ex)
                {
#if DEBUG
                    throw ex;
#endif
                    success = false;
                }
                
                if (permissions[Permission.Location] == PermissionStatus.Granted)
                {
                    ActivityGPS();
                }
                else
                {
                    try
                    {
                        var locationrequest = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                        if (locationrequest[Permission.Location] == PermissionStatus.Granted)
                        {
                            ActivityGPS();
                        }
                    }
                    catch { }
                }

                if(permissions.Count(e => e.Value == PermissionStatus.Granted) == permissions.Count)
                {

                }
                else
                {
                    var permisos = permissions.Where(e => e.Value != PermissionStatus.Granted).Select(e => e.Key).ToArray();
                    try
                    {
                        permissions = await CrossPermissions.Current.RequestPermissionsAsync(permisos);
                        success = true;
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        throw ex;
#endif
                        success = false;
                    }
                }

            }
            else if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                ActivityGPS();
            }

            ActivityGPS();

            App.Instance = this;
            App.RestClient = new RestForms();
            ShopList = new List<ShopItem>();

            if (Settings.Data.Count > 0)
            {
                string alias = Settings.Data.Get<string>("alias");
                string correo = Settings.Data.Get<string>("correo");
                int hasdirection = Settings.Data.Get<int>("hasdirection");
                int idperfil = Settings.Data.Get<int>("idperfil");
                int idusuario = Settings.Data.Get<int>("idusuario");
                string nombre = Settings.Data.Get<string>("nombre");
                string apellido = Settings.Data.Get<string>("apellido");
                string password = Settings.Data.Get<string>("password");
                string telefono = Settings.Data.Get<string>("telefono");
                int redtecnicos = Settings.Data.Get<int>("redtecnicos");
                string customerid = Settings.Data.Get<string>("customerid");

                OauthResult result = new OauthResult
                {
                    IdUsuario = idusuario,
                    Alias = alias,
                    Correo = correo,
                    HasDirection = hasdirection,
                    IdPerfil = idperfil,
                    Nombre = nombre,
                    Apellido = apellido,
                    Password = password,
                    Telefono = telefono,
                    CustomerId = customerid,
                    RedTecnicos = redtecnicos,
                };

                if (hasdirection > 0)
                {
                    Direction direction = new Direction();
                    direction.Ciudad = Settings.Data.Get<string>("ciudad");
                    direction.CodigoPostal = Settings.Data.Get<string>("codigopostal");
                    direction.Direccion = Settings.Data.Get<string>("direccion");
                    direction.Dnicuit = Settings.Data.Get<string>("dnicuit");
                    direction.Empresa = Settings.Data.Get<string>("empresa");
                    direction.Provincia = Settings.Data.Get<string>("provincia");
                    result.Direction = direction;
                }

                App.Oauth = result;

                if (result.IdPerfil == 4) // usuario final
                {
                    var navigationpage = new NavigationPage(new ProductsPage())
                    {
                        BarBackgroundColor = Color.FromHex("#8ea226"),
                        BarTextColor = Color.White
                    };
                    MainPage = navigationpage;
                    return;
                }
                else
                {
                    
                }
            }

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {

                var navigationpage = new NavigationPage(new ProductsPage())
                {
                    BarBackgroundColor = Color.FromHex("#8ea226"),
                    BarTextColor = Color.White
                };
                MainPage = navigationpage;
            }
            else
            {
                var navigationpage = new NavigationPage(new LoginPage())
                {
                    BarBackgroundColor = Color.FromHex("#8ea226"),
                    BarTextColor = Color.White
                };
                MainPage = navigationpage;
            }
        }

        private async void ActivityGPS()
        {
            App.GPS = GeoPosition.Instance;
            if (await App.GPS.PermitsGranted())
            {
                if (App.GPS.StartGPS())
                {
                    App.GPS.PositionChange += GPS_PositionChange;
                }
            }
        }

        private void GPS_PositionChange(object sender, Position e)
        {
            if (e.Success)
            {
                try
                {
                    Task.Run(async () =>
                    {
                        App.GPS.PositionChange -= GPS_PositionChange;
                        if (App.Oauth != null)
                        {
                            // position as string
                            var position = e.ToString();
                            // Update direction
                            var result = await App.RestClient.Post<UpdateResult>($"{App.BaseUrl}/User/UpdateGPS/{App.Oauth.IdUsuario}", new Dictionary<string, object>
                            {
                                { "GPS", position }
                            });

                            if (result != null)
                            {
                                if (result.UpdateStatus)
                                {
                                    App.GPS.PositionChange -= GPS_PositionChange;
                                    App.GPS.Stop();
                                    return;
                                }
                            }

                            App.GPS.PositionChange += GPS_PositionChange;
                        }
                    });
                }
                catch (Exception ex)
                {
#if DEBUG
                    throw ex;
#endif
                }
            }
        }
        
        public void SetMainPage(Page page, bool fullwindow = false)
        {
            if (fullwindow)
            {
                MainPage = page;
            }
            else
            {
                var navigationpage = new NavigationPage(page)
                {
                    BarBackgroundColor = Color.FromHex("#8ea226"),
                    BarTextColor = Color.White
                };
                MainPage = navigationpage;
            }
        }

        public static async void SetPreferences(OauthResult oauth)
        {
            if (oauth == null) return;
            Settings.Data.Replace = true;
            Settings.Data.Add("alias", oauth.Alias);
            Settings.Data.Add("correo", oauth.Correo);
            Settings.Data.Add("hasdirection", oauth.HasDirection);
            Settings.Data.Add("idperfil", oauth.IdPerfil);
            Settings.Data.Add("idusuario", oauth.IdUsuario);
            Settings.Data.Add("nombre", oauth.Nombre);
            Settings.Data.Add("apellido", oauth.Apellido);
            Settings.Data.Add("password", oauth.Password);
            Settings.Data.Add("telefono", oauth.Telefono);
            Settings.Data.Add("redtecnicos", oauth.RedTecnicos);
            Settings.Data.Add("customerid", oauth.CustomerId);
            if (oauth.HasDirection > 0 && oauth.Direction != null)
            {
                Settings.Data.Add("ciudad", oauth.Direction.Ciudad);
                Settings.Data.Add("codigopostal", oauth.Direction.CodigoPostal);
                Settings.Data.Add("direccion", oauth.Direction.Direccion);
                Settings.Data.Add("dnicuit", oauth.Direction.Dnicuit);
                Settings.Data.Add("empresa", oauth.Direction.Empresa);
                Settings.Data.Add("provincia", oauth.Direction.Provincia);
                await Settings.Data.SaveAsync();
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start("android=4987d005-e5a9-4adf-8456-05970dd2b461;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}",
                  typeof(Analytics), typeof(Crashes));

            CheckPermissions();
        }

        public static string ToJson()
        {
            return "";
        }

        protected override void OnSleep()
        {

        }

        protected override void OnResume()
        {

        }
    }
}
