using Plugin.Connectivity;
using ShoppingCart.API;
using ShoppingCart.API.Request;
using ShoppingCart.API.Result;
using ShoppingCart.Openpay;
using ShoppingCart.ORM;
using ShoppingCart.ViewModel;
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
    public partial class CarPage : ContentPage
    {
        public CarPage()
        {
            InitializeComponent();
            Init();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(App.Oauth != null && App.Oauth.Direction != null)
            {
                var direction = App.Oauth.Direction;
                BoxDirection.Text = $"{direction.Provincia}, {direction.Ciudad}, {direction.Direccion}, {direction.CodigoPostal}";
            }
        }
        
        private TicketRequest _request;
        private TicketResult _ticketstatus;

        private async void Init()
        {
            if (App.ShopList != null && App.ShopList.Count > 0)
            {
                var totalprice = 0d;
                int row = 0;
                CarTable.Children.Clear();
                EndContainer.Children.Clear();
                foreach (var shopitem in App.ShopList)
                {
                    string detail = "Cantidad: " + shopitem.Qty + ", Costo: $ ";
                    if (shopitem.Qty < int.Parse(shopitem.Product.Precio.CantidadMayoreo))
                    {
                        shopitem.Price = shopitem.Qty * double.Parse(shopitem.Product.Precio.PrecioUnitario, System.Globalization.NumberStyles.Any, App.Culture);
                        detail += shopitem.Product.Precio.PrecioUnitario;
                    }
                    else
                    {
                        shopitem.Price = shopitem.Qty * double.Parse(shopitem.Product.Precio.PrecioMayoreo, System.Globalization.NumberStyles.Any, App.Culture);
                        detail += shopitem.Product.Precio.PrecioMayoreo;
                    }
                    detail += ", Subtotal: $ " + shopitem.Price;
                    totalprice += shopitem.Price;

                    StackLayout layout = new StackLayout { Orientation = StackOrientation.Vertical };
                    Label productname = new Label { Text = "Producto: " + shopitem.Product.Nombre, TextColor = Color.Accent, FontAttributes = FontAttributes.Bold };
                    Label productdetail = new Label { Text = detail };
                    layout.Children.Add(productname);
                    layout.Children.Add(productdetail);

                    StackLayout editlayout = new StackLayout { Orientation = StackOrientation.Horizontal };
                    Entry productqtyedit = new Entry { Placeholder = shopitem.Qty.ToString(), Keyboard = Keyboard.Numeric, IsVisible = false, WidthRequest = 60 };
                    shopitem.BoxEdit = productqtyedit;
                    Button btnqtyget = new Button { Text = "Agregar", IsVisible = false, Command = new Command<ShopItem>(BtnQtyGet_Click), CommandParameter = shopitem };
                    shopitem.BtnEdit = btnqtyget;
                    Button btnqtycancel = new Button { Text = "Cancelar", IsVisible = false, Command = new Command<ShopItem>(BtnQtyCancel_Click), CommandParameter = shopitem };
                    shopitem.BtnCancel = btnqtycancel;

                    editlayout.Children.Add(productqtyedit);
                    editlayout.Children.Add(btnqtyget);
                    editlayout.Children.Add(btnqtycancel);

                    layout.Children.Add(editlayout);
                    
                    StackLayout layoutbuttons = new StackLayout { Orientation = StackOrientation.Horizontal };

                    Image edit = new Image { Source = "edit.png", WidthRequest = 30, HeightRequest = 30, Margin = new Thickness(5) };
                    edit.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command<ShopItem>(EditItem), CommandParameter = shopitem });

                    Image delete = new Image { Source = "trahs.png", WidthRequest = 30, HeightRequest = 30, Margin = new Thickness(5) };
                    delete.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command<ShopItem>(DeleteItem), CommandParameter = shopitem });

                    layoutbuttons.Children.Add(edit);
                    layoutbuttons.Children.Add(delete);

                    Grid.SetColumn(layout, 0);
                    Grid.SetColumn(layoutbuttons, 1);

                    CarTable.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    Grid.SetRow(layout, row);
                    Grid.SetRow(layoutbuttons, row);
                    
                    CarTable.Children.Add(layout);
                    CarTable.Children.Add(layoutbuttons);

                    row++;
                    //var textcell = new TextCell { Text = "Producto: " + shopitem.Product.Nombre, Detail = detail, DetailColor = Color.Gray };
                }

                /*
                if (ReferenceToCode != null)
                {
                    double descuento = 0;
                    if(double.TryParse(ReferenceToCode.Descuento, out descuento))
                    {
                        amountwithcode = totalprice = ((totalprice * (100 - descuento)) / 100);
                        EndContainer.Children.Add(new Label { HorizontalTextAlignment = TextAlignment.End, Text = "Precio con descuento: " + totalprice, FontAttributes = FontAttributes.Bold });
                    }
                    else
                    {
                        EndContainer.Children.Add(new Label { HorizontalTextAlignment = TextAlignment.End, Text = "Total: " + totalprice, FontAttributes = FontAttributes.Bold });
                    }
                }
                else
                {
                    EndContainer.Children.Add(new Label { HorizontalTextAlignment = TextAlignment.End, Text = "Total: " + totalprice, FontAttributes = FontAttributes.Bold });    
                }
                */
                EndContainer.Children.Add(new Label { HorizontalTextAlignment = TextAlignment.End, Text = "Total: " + totalprice, FontAttributes = FontAttributes.Bold });
            }
            else
            {
                await Navigation.PopAsync();
            }
        }
        
        private async void DeleteItem(ShopItem shopitem)
        {
            string cancelar = "Cancelar";
            string destruction = "Eliminar";
            var result = await DisplayActionSheet("¿Deseas eliminar este item?", cancelar, destruction);
            if (result == destruction)
            {
                App.ShopList.Remove(shopitem);
                Device.BeginInvokeOnMainThread(() =>
                {
                    Init();
                });
            }
        }

        private void EditItem(ShopItem shopitem)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                shopitem.BoxEdit.IsVisible = true;
                shopitem.BtnEdit.IsVisible = true;
                shopitem.BtnCancel.IsVisible = true;
            });
        }

        private void BtnQtyGet_Click(ShopItem shopitem)
        {
            if (shopitem.BoxEdit == null) return;
            if (App.Products == null) return;
            Device.BeginInvokeOnMainThread(async () =>
            {
                var shopitemidproduct = shopitem.IdProduct.ToString();
                var addtocar = App.Products.FirstOrDefault(e => e.Product.IdProducto == shopitemidproduct);
                if (addtocar == null) return;
                int qty = 0;
                if (int.TryParse(shopitem.BoxEdit.Text ?? "", out qty))
                {
                    if (qty < 1)
                    {
                        await DisplayAlert(App.AppName, "La cantidad debe ser mayor a cero", "Aceptar");
                    }
                    else
                    {
                        shopitem.BoxEdit.Text = "";
                        if (qty <= int.Parse(addtocar.Product.Stock.TotalStock))
                        {
                            int mayoreo = int.Parse(addtocar.Product.Precio.CantidadMayoreo);

                            double price = 0;
                            if (qty < mayoreo)
                            {
                                price = qty * double.Parse(addtocar.Product.Precio.PrecioUnitario, System.Globalization.NumberStyles.Any, App.Culture);
                            }
                            else
                            {
                                price = qty * double.Parse(addtocar.Product.Precio.PrecioMayoreo, System.Globalization.NumberStyles.Any, App.Culture);
                            }

                            var newinfoshop = new ShopItem
                            {
                                Qty = qty,
                                Price = price,
                                IdProduct = int.Parse(addtocar.Product.IdProducto),
                                Product = addtocar.Product
                            };

                            var shopiteminlist = App.ShopList.FirstOrDefault(item => item.Equals(newinfoshop));
                            if (shopiteminlist != null)
                            {
                                shopiteminlist.Qty = newinfoshop.Qty;
                                shopiteminlist.Price = newinfoshop.Price;
                                /*
                                var qtytemp = shopiteminlist.Qty + qty;
                                if (qtytemp > int.Parse(addtocar.Product.Stock.TotalStock))
                                {
                                    await DisplayAlert(App.AppName, "Sobrepasas la cantidad del stock, ingresa un número menor", "Aceptar");
                                    return;
                                }
                                shopiteminlist.Qty = qtytemp;
                                shopiteminlist.Price += price;
                                */
                            }
                            else
                            {
                                Init();
                                return;
                            }

                            await DisplayAlert(App.AppName, "Se edito correctamente el producto en el carrito", "Aceptar");
                            Init();
                        }
                        else
                        {
                            await DisplayAlert(App.AppName, "Por el momento no contamos con el número de articulos que deseas, en breve será surtido...", "Aceptar");
                        }
                    }
                }
                else
                {
                    await DisplayAlert(App.AppName, "Ingresa un número", "Aceptar");
                }
            });
        }

        private void BtnQtyCancel_Click(ShopItem shopitem)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Init();
            });
        }

        private async void BtnComprar_Clicked(object sender, EventArgs e)
        {
            BtnComprar.IsEnabled = false;
            if (App.Oauth == null)
            {
                await DisplayAlert(App.AppName, "Debes registrarte para poder comprar los productos", "Aceptar");
                try
                {
                    var result = await DisplayActionSheet(App.AppName, "Cancelar", null, "Registrarse");
                    if (!string.IsNullOrEmpty(result))
                    {
                        if (result.Equals("Registrarse"))
                        {
                            App.Instance.SetMainPage(new SIgninPage(), true);
                        }
                    }
                }
                catch
                {

                }
                return;
            }
            
            var direction = BoxDirection.Text ?? "";
            if (!string.IsNullOrEmpty(direction) || !string.IsNullOrWhiteSpace(direction))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Progress.IsVisible = true;
                    CompraContainer.IsVisible = false;
                    foreach (var shopitem in App.ShopList)
                    {
                        var result = await App.RestClient.Get<VerifyQtyResult>(App.BaseUrl + "/Stock/Verify", new Dictionary<string, object>
                        {
                            { "IdProducto", shopitem.IdProduct.ToString() },
                            { "Qty", shopitem.Qty.ToString() }
                        });

                        if (result != null)
                        {
                            if (result.Success == 0)
                            {
                                string message = "La cantidad que deseas comprar excede nuestro stock, nos hacen falta " + result.Qty + " para el producto " + shopitem.Product.Nombre;
                                await DisplayAlert(App.AppName, message, "Aceptar");
                                Progress.IsVisible = false;
                                CompraContainer.IsVisible = true;
                                return;
                            }
                        }
                        else
                        {
                            string message = "Intenta nuevamente por favor.";
                            await DisplayAlert(App.AppName, message, "Aceptar");
                            Progress.IsVisible = false;
                            CompraContainer.IsVisible = true;
                            return;
                        }
                    }

                    List<dynamic> sendtobuy = new List<dynamic>();
                    _request = new TicketRequest
                    {
                        IdUsuario = App.Oauth.IdUsuario,
                        IdCliente = App.IdCliente
                    };

                    /*
                    if (ReferenceToCode != null)
                    {
                        request.Total = amountwithcode;
                        request.IdCodigo = ReferenceToCode.IdCodigo;
                    }
                    */

                    double total = 0;
                    foreach (var shopitem in App.ShopList)
                    {
                        _request.Articulos.Add(new Articulo
                        {
                            IdProducto = shopitem.IdProduct,
                            Cantidad = shopitem.Qty,
                            Costo = shopitem.Price
                        });

                        total += shopitem.Price;
                    }

                    if (CrossConnectivity.Current.IsConnected)
                    {
                        _ticketstatus = await App.RestClient.Post<TicketResult, TicketRequest>(App.BaseUrl + "/Ticket/Add", _request);
                        if (_ticketstatus != null)
                        {
                            if (_ticketstatus.Code == 100)
                            {
                                if (MainPage.Instance != null) MainPage.Instance.SetTextBtnGoToShop("Items: 0, Total: $0");
                                // Limpiamos las listas
                                App.Products.Clear();
                                App.ShopList.Clear();
                                // Reseteamos el carrito
                                Init();
                                // Procesamos el pago...
                                ProcessPaymentPage paymentpage = new ProcessPaymentPage(total, $"DroidShop_{_ticketstatus.TicketNumber}", App.Oauth.CustomerId, App.BaseUrl);
                                paymentpage.ChargeResult += Paymentpage_ChargeResult;
                                paymentpage.CancelPayment += Paymentpage_CancelPayment;
                                await Navigation.PushAsync(paymentpage);
                            }
                            else
                            {
                                await DisplayAlert(App.AppName, "Ocurrio un error al realizar la venta, intenta más tarde", "Aceptar");
                                CompraContainer.IsVisible = true;
                                Progress.IsVisible = false;
                                BtnComprar.IsEnabled = true;
                            }
                        }
                        else
                        {
                            await DisplayAlert(App.AppName, "Ocurrio un error al realizar la venta, intenta más tarde", "Aceptar");
                            CompraContainer.IsVisible = true;
                            Progress.IsVisible = false;
                            BtnComprar.IsEnabled = true;
                        }
                    }
                    else
                    {
                        await DisplayAlert(App.AppName, "Necesitas conexion a internet para continuar", "Aceptar");
                        CompraContainer.IsVisible = true;
                        Progress.IsVisible = false;
                        BtnComprar.IsEnabled = true;
                    }
                });
            }
            else
            {
                await DisplayAlert(App.AppName, "Debes ingresar una direccion de envio", "Aceptar");
                CompraContainer.IsVisible = true;
                Progress.IsVisible = false;
                BtnComprar.IsEnabled = true;
            }
        }

        private async void Paymentpage_CancelPayment(object sender, EventArgs e)
        {
            try { await Navigation.PopAsync(); }catch { }
            if(_ticketstatus != null && _ticketstatus.Code == 100)
            {
                var result = await App.RestClient.Post<StandarResult>($"{App.BaseUrl}/Buy/DeleteAllTicket", new Dictionary<string, object>
                {
                    { "idticket", _ticketstatus.TicketNumber }
                });

                _ticketstatus = null;
            }
            try { await Navigation.PopAsync(); } catch { }
            CompraContainer.IsVisible = true;
            Progress.IsVisible = false;
            BtnComprar.IsEnabled = true;
        }

        private async void Paymentpage_ChargeResult(object sender, ChargeResult e)
        {
            try { await Navigation.PopAsync(); } catch { }
            if (_ticketstatus == null) return;
            await DisplayAlert(App.AppName, "Tu compra se realizo con exito, en breve nos pondremos en contacto contigo", "Aceptar");

            if (CrossConnectivity.Current.IsConnected)
            {
                var standarresult = await App.RestClient.Post<StandarResult>($"{App.BaseUrl}/Ticket/UpdateData/{_ticketstatus.TicketNumber}", new Dictionary<string, object>
                {
                    { "verificado", 1 },
                    { "notificacion", 3 }
                });

                if(standarresult != null)
                {
                    if(standarresult.Code == 100)
                    {
                        // se completo la compra
                    }
                    else
                    {
                        // debemos guardar para procesar mas tarde
                    }
                }
            }
            else
            {
                // debemos guardar para procesar despues...
            }

            try { await Navigation.PopAsync(); } catch { }

            CompraContainer.IsVisible = true;
            Progress.IsVisible = false;
            BtnComprar.IsEnabled = true;
        }

        /*
        public PromotionalCode ReferenceToCode { get; set; }

        private async void CodeExchange_Clicked(object sender, EventArgs e)
        {
            var searchcode = CodeText.Text ?? "";
            Device.BeginInvokeOnMainThread(() =>
            {
                (sender as Button).IsEnabled = false;
                CodeText.IsEnabled = false;
            });
            if (!string.IsNullOrEmpty(searchcode))
            {
                var result = await App.RestClient.Post<ApiResult<PromotionalCode>>($"{App.BaseUrl}/PromotionalCode/search", new Dictionary<string, object>
                {
                    { "Referencia", searchcode }
                });
                if (result != null)
                {
                    if (result.Result != null && result.Result.Count > 0)
                    {
                        ReferenceToCode = result.Result[0];
                        Init();
                    }
                    else
                    {
                        await DisplayAlert(App.AppName, "No encontramos ningun resultado con el código que has introducido", "Aceptar");
                    }
                }
                else
                {
                    // ocurrio un error en la api
                }
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                (sender as Button).IsEnabled = true;
                CodeText.IsEnabled = true;
                CodeText.Text = "";
            });
        }
        */
    }
}
