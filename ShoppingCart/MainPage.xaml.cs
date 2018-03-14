using ShoppingCart.API;
using ShoppingCart.API.Result;
using DevAzt.FormsX.Storage.Isolated;
using ShoppingCart.ORM;
using ShoppingCart.ViewModel;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShoppingCart
{
    public partial class MainPage : ContentPage
    {

        public Item _item { get; set; }
        public static MainPage Instance { get; set; }

        public MainPage(Item item = null)
        {
            InitializeComponent();
            Instance = this;
            if (App.Products == null || App.ShopList == null)
            {
                App.Products = new List<AddToCar>();
                App.ShopList = new List<ShopItem>();
            }
            _item = item;
            if (_item != null)
            {
                TagSearch_SearchButtonPressed(TagSearch, new EventArgs());
            }
        }

        public void SetTextBtnGoToShop(string text = "")
        {
            BtnGoToShop.Text = text;
        }

        private async void BtnGoToShop_Clicked(object sender, EventArgs e)
        {
            await this.GoToShop();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetPrice();
        }

        private async void BtnAddItem_Clicked(object args)
        {
            var addtocar = args as AddToCar;
            int qty = 0;
            if (int.TryParse(addtocar.BoxProducto.Text ?? "", out qty))
            {
                addtocar.BoxProducto.Text = "";
                if (!string.IsNullOrEmpty(addtocar.Product.Stock.TotalStock))
                {
                    if (qty <= int.Parse(addtocar.Product.Stock.TotalStock))
                    {
                        if (qty < 1)
                        {
                            await DisplayAlert(App.AppName, "La cantidad debe ser mayor a cero", "Aceptar");
                        }
                        else
                        {
                            int mayoreo = int.Parse(addtocar.Product.Precio.CantidadMayoreo);

                            double price = 0;
                            if (qty < mayoreo)
                            {
                                try
                                {
                                    price = qty * double.Parse(addtocar.Product.Precio.PrecioUnitario, System.Globalization.NumberStyles.Any, App.Culture);
                                }
                                catch
                                {
                                    
                                    return;
                                }
                            }
                            else
                            {
                                try
                                {
                                    price = qty * double.Parse(addtocar.Product.Precio.PrecioMayoreo, System.Globalization.NumberStyles.Any, App.Culture);
                                }
                                catch
                                {

                                }
                            }

                            var shopitem = new ShopItem
                            {
                                Qty = qty,
                                Price = price,
                                IdProduct = int.Parse(addtocar.Product.IdProducto),
                                Product = addtocar.Product
                            };

                            var shopiteminlist = App.ShopList.FirstOrDefault(item => item.Equals(shopitem));
                            if (shopiteminlist != null)
                            {
                                var qtytemp = shopiteminlist.Qty + qty;
                                if (qtytemp > int.Parse(addtocar.Product.Stock.TotalStock))
                                {
                                    await DisplayAlert(App.AppName, "Por el momento no contamos con el número de articulos que deseas, en breve será surtido...", "Aceptar");
                                    await NotifyZeroStock(shopitem.IdProduct, qtytemp);
                                    return;
                                }
                                shopiteminlist.Qty = qtytemp;
                                shopiteminlist.Price += price;
                            }
                            else
                            {
                                App.ShopList.Add(shopitem);
                            }

                            if (App.Products != null)
                            {
                                if (!App.Products.Contains(addtocar))
                                {
                                    App.Products.Add(addtocar);
                                }
                            }

                            SetPrice();
                            await DisplayAlert(App.AppName, "Se agrego el item al carrito", "Aceptar");
                        }
                    }
                    else
                    {
                        await DisplayAlert(App.AppName, "Por el momento no contamos con el número de articulos que deseas, en breve será surtido...", "Aceptar");
                        await NotifyZeroStock(int.Parse(addtocar.Product.IdProducto), qty);
                    }
                }
                else
                {
                    await DisplayAlert(App.AppName, "No tenemos stock para este producto", "Aceptar");
                    await NotifyZeroStock(int.Parse(addtocar.Product.IdProducto), qty);
                }
            }
            else
            {
                await DisplayAlert(App.AppName, "Ingresa un número", "Aceptar");
            }
        }

        private async Task NotifyZeroStock(int idproducto, int qty)
        {
            var result = await App.RestClient.Get<VerifyQtyResult>(App.BaseUrl + "/Stock/Verify", new Dictionary<string, object>
            {
                { "IdProducto", idproducto.ToString() },
                { "Qty", qty.ToString() }
            });

            if (result != null)
            {

            }
        }

        private void SetPrice()
        {
            if (App.ShopList == null || App.Products == null)
            {
                string textbutton = "Items: 0, Total: $0";
                Device.BeginInvokeOnMainThread(() =>
                {
                    BtnGoToShop.Text = textbutton;
                });
            }
            else
            {
                var qtytotal = App.ShopList.Sum(s => s.Qty);
                var pricetotal = App.ShopList.Sum(s => s.Price);
                string textbutton = string.Format("Items: {0}, Total: ${1}", qtytotal, pricetotal);
                Device.BeginInvokeOnMainThread(() =>
                {
                    BtnGoToShop.Text = textbutton;
                });
            }
        }
        
        private void TagSearch_SearchButtonPressed(object sender, EventArgs e)
        {
            // Se ha quitado la inicialización de las listas...
            Device.BeginInvokeOnMainThread(async () =>
            {
                RootLayout.Children.Clear();
                ProgressElement.IsVisible = true;
                ProgressElement.IsRunning = true;

                var texttosearch = TagSearch.Text ?? "";

                if(_item != null)
                {
                    texttosearch = TagSearch.Text = _item.Title;
                }

                if (!string.IsNullOrEmpty(texttosearch))
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {


                        SearchResult result = null;
                        if (_item != null)
                        {
                            string column = "";
                            if (_item.Data is Categoria)
                            {
                                column = "idcategoria";
                            }
                            else if (_item.Data is Modelo)
                            {
                                column = "idmodelo";
                            }

                            result = await App.RestClient.Get<SearchResult>(App.BaseUrl + "/Product/Search", new Dictionary<string, object>
                        {
                            { column, _item.Id }
                        });
                        }
                        else
                        {
                            result = await App.RestClient.Get<SearchResult>(App.BaseUrl + "/Product/Search", new Dictionary<string, object>
                        {
                            { "tag", texttosearch }
                        });
                        }

                        if (result != null)
                        {
                            if (result.Code == 100)
                            {
                                foreach (var producto in result.Productos)
                                {
                                    // StackLayout horizontallayout = new StackLayout { Orientation = StackOrientation.Horizontal };

                                    Image image = new Image { Source = producto.ImageUrl, HeightRequest = 150, Aspect = Aspect.AspectFit, HorizontalOptions = LayoutOptions.Center };

                                    StackLayout contentlayout = new StackLayout { Padding = new Thickness(10), Orientation = StackOrientation.Vertical };
                                    StackLayout detailsimagelayout = new StackLayout { Orientation = StackOrientation.Vertical };
                                    StackLayout detailslayout = new StackLayout { Orientation = StackOrientation.Vertical };
                                    Label title = new Label { FontSize = 16, Text = producto.Nombre, FontAttributes = FontAttributes.Bold };
                                    detailslayout.Children.Add(title);
                                    Label description = new Label { FontSize = 12, Text = producto.Parte };
                                    detailslayout.Children.Add(description);
                                    var precio = "0";
                                    if (producto.Precio != null)
                                    {
                                        precio = producto.Precio.PrecioUnitario != null ? producto.Precio.PrecioUnitario.ToString() : "No hay precio";
                                    }

                                    if (precio == "0")
                                    {

                                    }
                                    else
                                    {
                                        Label color = new Label { VerticalTextAlignment = TextAlignment.Center, FontSize = 14, Text = string.Format("Precio: ${0}", precio) };
                                        detailsimagelayout.Children.Add(detailslayout);
                                        StackLayout formadd = new StackLayout { Orientation = StackOrientation.Horizontal };
                                        Entry cantidad = new Entry { WidthRequest = 60, Placeholder = "0", Keyboard = Keyboard.Numeric };
                                        Button agregar = new Button { Text = "Agregar" };
                                        AddToCar car = new AddToCar
                                        {
                                            BoxProducto = cantidad,
                                            Product = producto
                                        };
                                        agregar.Command = new Command(BtnAddItem_Clicked);
                                        agregar.CommandParameter = car;
                                        formadd.Children.Add(color);
                                        formadd.Children.Add(cantidad);
                                        formadd.Children.Add(agregar);
                                        contentlayout.Children.Add(image);
                                        contentlayout.Children.Add(detailsimagelayout);
                                        contentlayout.Children.Add(formadd);
                                        contentlayout.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(ContentTap), CommandParameter = producto });
                                        RootLayout.Children.Add(contentlayout);
                                    }
                                }
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        await DisplayAlert(App.AppName, "Revisa tu conexion a internet", "Aceptar");
                    }
                }
                else
                {
                    await DisplayAlert(App.AppName, "Ingresa un texto a buscar", "Aceptar");
                }
                ProgressElement.IsVisible = false;
                ProgressElement.IsRunning = false;
            });
        }

        private async void ContentTap(object obj)
        {
            if (obj is Producto producto)
            {
                await Navigation.PushAsync(new ImagePage(producto) { Title = producto.Nombre });
            }
        }
    }
}
