using DevAzt.FormsX.Device.Sensors;
using ShoppingCart.API;
using ShoppingCart.API.Exceptions;
using ShoppingCart.API.Result;
using DevAzt.FormsX.Net.HttpClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShoppingCart
{

    public enum LogisticType
    {
        Capital, Interior
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TicketPage : ContentPage
    {

        private TicketsResult Result { get; set; }
        private OauthResult Oauth { get; set; }
        private LogisticType Type { get; set; }

        public TicketPage(OauthResult oauth, LogisticType type = LogisticType.Interior, List<Ticket> tickets = null)
        {
            Oauth = oauth;
            Type = type;
            InitializeComponent();
            InitTicket(oauth, type, tickets);
        }
        
        private async void InitTicket(OauthResult oauth, LogisticType type, List<Ticket> tickets)
        {
            if (oauth != null)
            {
                if (oauth.IdPerfil == 4) // cliente
                {
                    if (ToolbarItems.Count == 0)
                    {
                        ToolbarItems.Add(new ToolbarItem { Text = "Refrescar", Order = ToolbarItemOrder.Primary, Priority = 1, Command = new Command(Refresh) });
                    }
                    await SetClientView(oauth);
                }
                else if(oauth.IdPerfil == 1) // administrador
                {
                    await SetAdminView(oauth, tickets);
                    if (ToolbarItems.Count == 0)
                    {
                        ToolbarItems.Add(new ToolbarItem { Text = "Refrescar", Order = ToolbarItemOrder.Primary, Priority = 1, Command = new Command(Refresh) });
                        ToolbarItems.Add(new ToolbarItem { Text = "Buscar", Order = ToolbarItemOrder.Secondary, Priority = 0, Command = new Command(FindTicket) });
                    }
                }
                else if (oauth.IdPerfil == 2) // repartidor
                {
                    if (ToolbarItems.Count == 0)
                    {
                        ToolbarItems.Add(new ToolbarItem { Text = "Refrescar", Order = ToolbarItemOrder.Primary, Priority = 1, Command = new Command(Refresh) });
                        ToolbarItems.Add(new ToolbarItem { Text = "Capital Federal", Order = ToolbarItemOrder.Secondary, Priority = 0, Command = new Command<LogisticType>(ChangePage), CommandParameter = LogisticType.Capital });
                        ToolbarItems.Add(new ToolbarItem { Text = "Interior", Order = ToolbarItemOrder.Secondary, Priority = 0, Command = new Command<LogisticType>(ChangePage), CommandParameter = LogisticType.Interior });
                        ToolbarItems.Add(new ToolbarItem { Text = "Buscar", Order = ToolbarItemOrder.Secondary, Priority = 0, Command = new Command(FindTicket) });
                    }
                    await SetLocalView(oauth, type, tickets);
                }
            }
        }

        private void Refresh(object obj)
        {
            InitTicket(Oauth, Type, null);
        }

        private async void FindTicket(object obj)
        {
            await Navigation.PushAsync(new FilterPage ());
        }

        private async void ChangePage(LogisticType type)
        {
            await Navigation.PushAsync(new TicketPage(Oauth, type));
        }

        #region Distribuidor

        private async Task SetLocalView(OauthResult oauth, LogisticType type, List<Ticket> tickets)
        {
            try
            {
                TicketsTable.Children.Clear();
                if (tickets == null)
                {
                    var hoy = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd");
                    var pasado = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
                    Result = await App.RestClient.Get<TicketsResult>(App.BaseUrl + "/Ticket/OfLogistic", new Dictionary<string, object>
                    {
                        { "fechainicial", pasado },
                        { "fechafinal", hoy }
                    });

                    if (Result != null && Result.Code == 100 && Result.Tickets != null && Result.Tickets.Count > 0) // mostramos las compras del usuario
                    {
                        tickets = Result.Tickets.OrderByDescending(e => e.Baucher).ToList();
                        SetTicketsLocalView(type, tickets, true);
                    }
                    else
                    {
                        await DisplayAlert(App.AppName, "No encontramos información para tu usuario, intente más tarde...", "Aceptar");
                    }
                }
                else
                {
                    SetTicketsLocalView(type, tickets, false);
                }
            }
            catch
            {
                await DisplayAlert(App.AppName, "Verifica tu conexión a internet", "Aceptar");
            }
        }

        private void SetTicketsLocalView(LogisticType type, List<Ticket> tickets, bool filter = true)
        {
            TicketsTable.RowDefinitions.Add(new RowDefinition { Height = 40 });
            Label pedido = new Label { Text = "Pedido" };
            Label estatus = new Label { Text = "Estatus" };
            Label monto = new Label { Text = "Monto" };
            Label comprobante = new Label { Text = "Subir recibos" };

            if (type == LogisticType.Interior)
            {
                monto.Text = "Transporte";
                comprobante.Text = "Guia";
            }
            else if (type == LogisticType.Capital)
            {
                comprobante.Text = "Cobrado";
            }

            SetRowAndColumn(pedido, estatus, monto, comprobante, null, 0, null);

            int row = 1;
            foreach (var ticket in tickets)
            {
                string cliente = "";
                if (ticket.Usuario != null)
                {
                    cliente = ticket.Usuario.Nombre ?? ticket.Usuario.Correo;
                }
                pedido = new Label { Text = cliente };
                pedido.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command<OauthResult>(InfoClient), CommandParameter = ticket.Usuario });
                bool pagado = false;
                string state = "Sin pagar";
                if (ticket.Baucher == null)
                {
                    state = "Sin pagar";
                }
                else if (ticket.Baucher != null && ticket.Verificado.Equals("0"))
                {
                    state = "En revisión";
                }
                else if (ticket.Baucher != null && ticket.Verificado.Equals("1") && ticket.Paqueteria == null)
                {
                    pagado = true;
                    state = "Pagado";
                }
                else
                {
                    pagado = true;
                    state = "Pagado";
                }
                estatus = new Label { Text = state };

                var empresa = ticket.Direccion != null ? ticket.Direccion.Empresa ?? "Sin dirección" : "Sin dirección";

                if (type == LogisticType.Interior)
                {
                    monto = new Label { Text = empresa };
                    if (!empresa.Equals("Sin dirección"))
                    {
                        monto.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command<Ticket>(ShowDirection), CommandParameter = ticket });
                    }
                }
                else if (type == LogisticType.Capital)
                {
                    monto = new Label { Text = "$" + ticket.Total };
                }

                ticket.Pedido = pedido;
                ticket.Estatus = estatus;
                ticket.Monto = monto;


                if (type == LogisticType.Interior)
                {
                    StackLayout layoutbuttons = new StackLayout { Orientation = StackOrientation.Horizontal };
                    Image camera = new Image { Source = "camera.png", WidthRequest = 25, HeightRequest = 25, Margin = new Thickness(5) };
                    bool enviado = false;
                    if (ticket.Paqueteria == null)
                    {
                        if (pagado)
                        {
                            camera.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command<Ticket>(AddGuia), CommandParameter = ticket });
                        }
                        else
                        {
                            camera.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command<Ticket>(WhitoutPayment), CommandParameter = ticket });
                        }
                    }
                    else
                    {
                        enviado = true;
                        camera.Source = "document.png";
                        camera.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command<Ticket>(ViewPaqueteria), CommandParameter = ticket });
                    }
                    ticket.BtnCamara = camera;
                    layoutbuttons.Children.Add(camera);
                    if (filter)
                    {
                        if (!enviado)
                        {
                            SetRowAndColumn(pedido, estatus, monto, null, layoutbuttons, row, ticket);
                            row++;
                        }
                    }
                    else
                    {
                        SetRowAndColumn(pedido, estatus, monto, null, layoutbuttons, row, ticket);
                        row++;
                    }
                }
                else if (type == LogisticType.Capital)
                {
                    StackLayout layoutbuttons = new StackLayout { Orientation = StackOrientation.Horizontal };
                    if (!pagado)
                    {
                        Image camera = new Image { Source = "like.png", WidthRequest = 25, HeightRequest = 25, Margin = new Thickness(5) };
                        camera.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command<Ticket>(ConfirmarPago), CommandParameter = ticket });
                        ticket.BtnCamara = camera;
                        layoutbuttons.Children.Add(camera);
                    }

                    if (!empresa.Equals("Sin dirección"))
                    {
                        Image direccion = new Image { Source = "map.png", WidthRequest = 25, HeightRequest = 25, Margin = new Thickness(5) };
                        direccion.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command<Direction>(CheckAddress), CommandParameter = ticket.Direccion });
                        ticket.BtnDireccion = direccion;
                        layoutbuttons.Children.Add(direccion);
                    }

                    if (filter)
                    {
                        if (!pagado)
                        {
                            SetRowAndColumn(pedido, estatus, monto, null, layoutbuttons, row, ticket);
                            row++;
                        }
                    }
                    else
                    {
                        SetRowAndColumn(pedido, estatus, monto, null, layoutbuttons, row, ticket);
                        row++;
                    }
                }
            }
        }

        private void InfoClient(OauthResult oauth)
        {

        }

        private async void CheckAddress(Direction direction)
        {
            if (direction != null)
            {
                await DisplayAlert(App.AppName, direction.ToString(), "Aceptar");
            }
        }

        private async void ShowDirection(Ticket ticket)
        {
            await Navigation.PushAsync(new DirectionPage(ticket.Direccion));
        }

        #endregion

        #region Admin

        private async Task SetAdminView(OauthResult oauth, List<Ticket> tickets = null)
        {
            try
            {
                TicketsTable.Children.Clear();
                if (tickets == null)
                {
                    var hoy = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd");
                    var pasado = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
                    Result = await App.RestClient.Get<TicketsResult>(App.BaseUrl + "/Ticket/OfAdmin", new Dictionary<string, object>
                    {
                        { "fechainicial", pasado },
                        { "fechafinal", hoy }
                    });
                    if (Result != null && Result.Code == 100 && Result.Tickets != null && Result.Tickets.Count > 0) // mostramos las compras del usuario
                    {
                        tickets = Result.Tickets;
                        SetTicketsAdminView(tickets, true);
                    }
                    else
                    {
                        await DisplayAlert(App.AppName, "No encontramos información para tu usuario, intente más tarde...", "Aceptar");
                    }
                }
                else
                {
                    SetTicketsAdminView(tickets, false);
                }
            }
            catch
            {
                await DisplayAlert(App.AppName, "Verifica tu conexión a internet", "Aceptar");
            }
        }

        private void SetTicketsAdminView(List<Ticket> tickets = null, bool filter = true)
        {
            TicketsTable.RowDefinitions.Add(new RowDefinition { Height = 40 });

            Label pedido = new Label { Text = "Cliente" };
            Label estatus = new Label { Text = "Estatus" };
            Label monto = new Label { Text = "Monto" };
            Label comprobante = new Label { Text = "Verificar" };
            
            SetRowAndColumn(pedido, estatus, monto, comprobante, null, 0, null);

            int row = 1;
            foreach (var ticket in tickets)
            {
                string cliente = "";
                if (ticket.Usuario != null)
                {
                    cliente = ticket.Usuario.Nombre ?? ticket.Usuario.Correo;
                }

                pedido = new Label { Text = cliente };
                pedido.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command<OauthResult>(InfoClient), CommandParameter = ticket.Usuario });
                bool boucher = true;
                bool pagado = false;
                string state = "Sin pagar";
                if (ticket.Baucher == null)
                {
                    boucher = false;
                    state = "Sin pagar";
                }
                else if (ticket.Baucher != null && ticket.Verificado.Equals("0"))
                {
                    state = "Verificar";
                }
                else if (ticket.Baucher != null && ticket.Verificado.Equals("1") && ticket.Paqueteria == null)
                {
                    pagado = true;
                    state = "Pagado";
                }
                else
                {
                    pagado = true;
                    state = "Pagado";
                }
                estatus = new Label { Text = state };

                var empresa = ticket.Direccion != null ? ticket.Direccion.Empresa : "Sin dirección";
                monto = new Label { Text = "$" + ticket.Total };
                ticket.Pedido = pedido;
                ticket.Estatus = estatus;
                ticket.Monto = monto;


                StackLayout layoutbuttons = new StackLayout { Orientation = StackOrientation.Horizontal };

                if (boucher)
                {
                    Image camera = new Image { Source = "document.png", WidthRequest = 25, HeightRequest = 25, Margin = new Thickness(5) };
                    camera.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command<Ticket>(ViewBaucher), CommandParameter = ticket });
                    ticket.BtnCamara = camera;
                    layoutbuttons.Children.Add(camera);

                    if (!pagado)
                    {
                        Image like = new Image { Source = "like.png", WidthRequest = 25, HeightRequest = 25, Margin = new Thickness(5) };
                        like.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command<Ticket>(ConfirmarPagoAdmin), CommandParameter = ticket });
                        ticket.BtnLike = like;
                        layoutbuttons.Children.Add(like);
                    }
                }

                if (filter)
                {
                    if (!pagado)
                    {
                        SetRowAndColumn(pedido, estatus, monto, null, layoutbuttons, row, ticket);
                        row++;
                    }
                }
                else
                {
                    SetRowAndColumn(pedido, estatus, monto, null, layoutbuttons, row, ticket);
                    row++;
                }
            }
        }

        private void ConfirmarPagoAdmin(Ticket ticket)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                ProgressElement.IsVisible = true;
                TicketsTable.IsVisible = false;
                ProgressText.Text = "Verificando el pago";
                var result = await App.RestClient.Post<StandarResult>(App.BaseUrl + "/Ticket/UpdateVerificadoAdmin", new Dictionary<string, object>
                {
                    { "idticket", ticket.IdTicket },
                    { "fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                });

                if (result != null && result.Code == 100)
                {
                    ticket.Estatus.Text = "Pagado";
                    ticket.BtnLike.GestureRecognizers.Clear();
                    ticket.BtnLike.IsVisible = false;
                    await DisplayAlert(App.AppName, "Se verifico el pago", "Aceptar");
                }
                else
                {
                    await DisplayAlert(App.AppName, "No pudimos verificar el pago, intentalo de nuevo", "Aceptar");
                }
                ProgressElement.IsVisible = false;
                TicketsTable.IsVisible = true;
            });
        }

        #endregion

        #region Client

        private async Task SetClientView(OauthResult oauth)
        {
            try
            {
                TicketsTable.Children.Clear();
                Result = await App.RestClient.Get<TicketsResult>(App.BaseUrl + "/Ticket/OfClient?idusuario=" + oauth.IdUsuario, new Dictionary<string, object> { });
                if (Result != null && Result.Code == 100 && Result.Tickets != null && Result.Tickets.Count > 0) // mostramos las compras del usuario
                {
                    var tickets = Result.Tickets;

                    TicketsTable.RowDefinitions.Add(new RowDefinition { Height = 40 });

                    Label pedido = new Label { Text = "Pedido" };
                    Label estatus = new Label { Text = "Estatus" };
                    Label monto = new Label { Text = "Monto" };
                    Label comprobante = new Label { Text = "Subir recibos" };

                    SetRowAndColumn(pedido, estatus, monto, comprobante, null, 0, null);
                    int row = 1;
                    foreach (var ticket in tickets)
                    {
                        pedido = new Label { Text = ticket.IdTicket.ToString() };
                        string state = "Subir baucher";
                        if (ticket.Baucher == null)
                        {
                            state = "Subir baucher";
                        }
                        else if (ticket.Baucher != null && ticket.Verificado.Equals("0"))
                        {
                            state = "En revisión";
                        }
                        else if (ticket.Baucher != null && ticket.Verificado.Equals("1") && ticket.Paqueteria == null)
                        {
                            state = "Verificado";
                        }
                        else
                        {
                            state = "Enviado";
                        }
                        estatus = new Label { Text = state };

                        monto = new Label { Text = "$" + ticket.Total };

                        ticket.Pedido = pedido;
                        ticket.Estatus = estatus;
                        ticket.Monto = monto;

                        StackLayout layoutbuttons = new StackLayout { Orientation = StackOrientation.Horizontal };
                        Image camera = new Image { Source = "camera.png", WidthRequest = 25, HeightRequest = 25, Margin = new Thickness(5) };
                        if (!state.Equals("Subir baucher"))
                        {
                            camera.Source = "document.png";
                        }
                        
                        if (ticket.Baucher == null)
                        {
                            camera.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command<Ticket>(AddBaucher), CommandParameter = ticket });
                        }
                        else
                        {
                            camera.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command<Ticket>(ViewBaucher), CommandParameter = ticket });
                        }
                        ticket.BtnCamara = camera;
                        Image air = new Image { Source = "map.png", WidthRequest = 25, HeightRequest = 25, Margin = new Thickness(5) };
                        air.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command<Ticket>(ViewPaqueteria), CommandParameter = ticket });
                        layoutbuttons.Children.Add(camera);
                        layoutbuttons.Children.Add(air);
                        SetRowAndColumn(pedido, estatus, monto, null, layoutbuttons, row, ticket);
                        row++;
                    }
                }
                else
                {
                    await DisplayAlert(App.AppName, "No encontramos información para tu usuario, intenta más tarde...", "Aceptar");
                }
            }
            catch
            {
                await DisplayAlert(App.AppName, "Verifica tu conexión a internet", "Aceptar");
            }
        }

        #endregion

        private void SetRowAndColumn(Label pedido, Label estatus, Label monto, Label comprobante, StackLayout layout, int row, Ticket ticket)
        {
            if (ticket != null)
            {
                pedido.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command<Ticket>(ViewTicket), CommandParameter = ticket });
                estatus.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command<Ticket>(ViewTicket), CommandParameter = ticket });
                monto.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command<Ticket>(ViewTicket), CommandParameter = ticket });
            }

            TicketsTable.RowDefinitions.Add(new RowDefinition { Height = 40 });
            pedido.VerticalTextAlignment = estatus.VerticalTextAlignment = monto.VerticalTextAlignment = TextAlignment.Center;
            Grid.SetColumn(pedido, 0);
            Grid.SetColumn(estatus, 1);
            Grid.SetColumn(monto, 2);
            if (comprobante != null)
            {
                comprobante.VerticalTextAlignment = TextAlignment.Center;
                Grid.SetColumn(comprobante, 3);
            }
            else if(layout != null)
            {
                Grid.SetColumn(layout, 3);
            }

            Grid.SetRow(pedido, row);
            Grid.SetRow(estatus, row);
            Grid.SetRow(monto, row);

            if (comprobante != null)
            {
                Grid.SetRow(comprobante, row);
            }
            else if (layout != null)
            {
                Grid.SetRow(layout, row);
            }

            TicketsTable.Children.Add(pedido);
            TicketsTable.Children.Add(estatus);
            TicketsTable.Children.Add(monto);

            if (comprobante != null)
            {
                TicketsTable.Children.Add(comprobante);
            }else if (layout != null)
            {
                TicketsTable.Children.Add(layout);
            }
        }

        private async void ViewTicket(Ticket ticket)
        {
            await Navigation.PushAsync(new InfoTicket(ticket));
        }

        private void ConfirmarPago(Ticket ticket)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                ProgressElement.IsVisible = true;
                TicketsTable.IsVisible = false;
                ProgressText.Text = "Verificando el pago";
                var result = await App.RestClient.Post<StandarResult>(App.BaseUrl + "/Ticket/UpdateVerificadoLogistic", new Dictionary<string, object>
                {
                    { "idticket", ticket.IdTicket },
                    { "fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                });

                if (result != null && result.Code == 100)
                {
                    ticket.Estatus.Text = "Pagado";
                    ticket.Baucher = "default.jpg";
                    ticket.BtnCamara.GestureRecognizers.Clear();
                    ticket.BtnCamara.IsVisible = false;
                    await DisplayAlert(App.AppName, "Se verifico el pago", "Aceptar");
                    InitTicket(Oauth, Type, null);
                }
                else
                {
                    await DisplayAlert(App.AppName, "No pudimos verificar el pago, intentalo de nuevo", "Aceptar");
                }
                ProgressElement.IsVisible = false;
                TicketsTable.IsVisible = true;
            });
        }

        private async void AddGuia(Ticket ticket)
        {
            CameraCaptureTask task = new CameraCaptureTask
            {
                FileName = "guiano" + ticket.IdTicket + ".jpg",
                FolderName = "Photos"
            };

            var result = await task.TakePhoto();
            if (!result.Success) // ocurrio algun error
            {
                await DisplayAlert(App.AppName, result.Message, "Aceptar");
            }
            else if (result.Photo != null) // se tomo la foto con exito
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    ProgressElement.IsVisible = true;
                    TicketsTable.IsVisible = false;
                    ProgressText.Text = "Subiendo guia de transporte, espere...";
                    var paramsdata = new List<Param>()
                    {
                        new Param ("image", result.Photo.GetStream(), ParamType.File, task.FileName),
                        new Param ("idticket", ticket.IdTicket, ParamType.String),
                        new Param ("fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ParamType.String)
                    };
                    var paqueteriaresult = await App.RestClient.Post<PaqueteriaResult>(App.BaseUrl + "/Ticket/UpdateGuia", complexparams: paramsdata);
                    if (paqueteriaresult != null && paqueteriaresult.Status == 100)
                    {
                        ticket.Estatus.Text = "Enviado";
                        ticket.Paqueteria = paqueteriaresult.FileName;
                        ticket.BtnCamara.GestureRecognizers.Clear();
                        ticket.BtnCamara.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command<Ticket>(ViewPaqueteria), CommandParameter = ticket });
                        await DisplayAlert(App.AppName, "Se ha guardado la guia", "Aceptar");
                        InitTicket(Oauth, Type, null);
                    }
                    else
                    {
                        await DisplayAlert(App.AppName, "No pudimos guardar la guia, intentalo de nuevo", "Aceptar");
                    }
                    ProgressElement.IsVisible = false;
                    TicketsTable.IsVisible = true;
                });
            }
        }

        private async void ViewPaqueteria(Ticket ticket)
        {
            if (!string.IsNullOrEmpty(ticket.Paqueteria))
            {
                await Navigation.PushAsync(new ImagePage(new Producto { Image = ticket.Paqueteria, Nombre = "Pedido # " + ticket.IdTicket, Parte = "Costo $ " + ticket.Total }));
            }
            else
            {
                await DisplayAlert(App.AppName, "Aun no se ha subido la foto de la paqueteria", "Aceptar");
            }
        }

        private async void WhitoutPayment(Ticket ticket)
        {
           await DisplayAlert(App.AppName, "Aun no se a realizado un pago para la orden " + ticket.IdTicket, "Aceptar");
        }

        private async void AddBaucher(Ticket ticket)
        {
            CameraCaptureTask task = new CameraCaptureTask
            {
                FileName = "baucherorderno" + ticket.IdTicket + ".jpg",
                FolderName = "Photos"
            };

            var result = await task.TakePhoto();
            if (!result.Success) // ocurrio algun error
            {
                await DisplayAlert(App.AppName, result.Message, "Aceptar");
            }
            else if (result.Photo != null) // se tomo la foto con exito
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    ProgressElement.IsVisible = true;
                    TicketsTable.IsVisible = false;
                    ProgressText.Text = "Subiendo boucher de pago, espere...";
                    var paramsdata = new List<Param>()
                    {
                        new Param ("idticket", ticket.IdTicket, ParamType.String),
                        new Param ("fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ParamType.String),
                        new Param ("image", result.Photo.GetStream(), ParamType.File, task.FileName, "image/jpeg")
                    };
                    var boucherresult = await App.RestClient.Post<BoucherResult>(App.BaseUrl + "/Ticket/UpdateBoucher", complexparams: paramsdata);
                    if (boucherresult != null && boucherresult.Status == 100)
                    {
                        ticket.Estatus.Text = "En revisión";
                        ticket.Baucher = boucherresult.BoucherName;
                        ticket.BtnCamara.GestureRecognizers.Clear();
                        ticket.BtnCamara.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command<Ticket>(ViewBaucher), CommandParameter = ticket });
                        ticket.BtnCamara.Source = "document.png";
                        await DisplayAlert(App.AppName, "Se ha guardado el boucher", "Aceptar");
                    }
                    else
                    {
                        await DisplayAlert(App.AppName, "No pudimos guardar el boucher, intentalo de nuevo\nNota: baja la resolución de la camara para un mejor resultado...", "Aceptar");
                    }
                    ProgressElement.IsVisible = false;
                    TicketsTable.IsVisible = true;
                });
            }
        }

        private async void ViewBaucher(Ticket ticket)
        {
            if (!string.IsNullOrEmpty(ticket.Baucher))
            {
                await Navigation.PushAsync(new ImagePage(new Producto { Image = ticket.Baucher, Nombre = "Pedido # " + ticket.IdTicket, Parte = "Costo $ " + ticket.Total }));
            }
            else
            {
                await DisplayAlert(App.AppName, "Aún no se ha subido la foto del boucher", "Aceptar");
            }
        }
    }
}
