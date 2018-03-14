using DevAzt.FormsX.Storage.Isolated;
using ShoppingCart.ORM;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShoppingCart
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductsPage : TabbedPage
    {
        public ProductsPage()
        {
            InitializeComponent();
            InitSections();
        }

        private async void InitSections()
        {
            // init marcas
            ObservableCollection<Item> items = new ObservableCollection<Item>();
            var listmarcas = new SimpleListPage(items) { Title = "Marcas", RowHeight = 80 };
            listmarcas.ItemSelected += Listmarcas_ItemSelected;
            Children.Add(listmarcas);
            ObservableCollection<Item> itemscategorias = new ObservableCollection<Item>();
            var categorias = new SimpleListPage(itemscategorias) { Title = "Categorias", RowHeight = 80 };
            categorias.ItemSelected += Categorias_ItemSelected;
            Children.Add(categorias);
            var mainpage = new MainPage() { Title = "Busqueda" };
            Children.Add(mainpage);

            if (CrossConnectivity.Current.IsConnected)
            {
                var catalogos = await App.RestClient.Get<Catalogos>($"{App.BaseUrl}/Catalogo/get/{App.IdCliente}", new Dictionary<string, object>());
                if (catalogos != null)
                {
                    if (catalogos.Marcas != null && catalogos.Marcas.Count > 0)
                    {
                        SetListMarcas(catalogos.Marcas, catalogos.Modelos, items);
                    }

                    if (catalogos.Categorias != null && catalogos.Categorias.Count > 0)
                    {
                        SetCategorias(catalogos.Categorias, itemscategorias);
                    }
                }
                else
                {
                    await DisplayAlert(App.AppName, "No pudimos contactar con la tienda, inteta más tarde", "Aceptar");
                }
            }
            else
            {
                await DisplayAlert(App.AppName, "Verifica tu conexión a internet", "Aceptar");
            }
        }

        private void SetCategorias(List<Categoria> categorias, ObservableCollection<Item> items)
        {
            categorias = categorias.Where(e => !e.Nombre.Contains("script")).OrderBy(e => e.Nombre).ToList();
            foreach (var categoria in categorias)
            {
                Item item = new Item()
                {
                    Id = int.Parse(categoria.IdCategoria),
                    Description = "Haz click aqui para ver los modelos de la marca",
                    Title = categoria.Nombre,
                    Data = categoria,
                    Image = categoria.ImageUrl
                };
                items.Add(item);
            }
        }

        private void SetListMarcas(List<Marca> marcas, List<Modelo> modelos, ObservableCollection<Item> items)
        {
            marcas = marcas.Where(e => !e.Nombre.Contains("script")).OrderBy(e => e.Nombre).ToList();
            foreach (var marca in marcas)
            {
                Item item = new Item()
                {
                    Id = int.Parse(marca.IdMarca),
                    Description = "Haz click aqui para ver los modelos de la marca",
                    Title = marca.Nombre,
                    Image = marca.ImageUrl
                };

                if (modelos != null && modelos.Count() > 0)
                {
                    item.Childs = new ObservableCollection<Item>();
                    var filtro = modelos.Where(e => e.IdMarca == marca.IdMarca).OrderBy(e => e.Nombre).ToList();
                    foreach (var filtromodelo in filtro)
                    {
                        item.Childs.Add(new Item
                        {
                            Id = int.Parse(filtromodelo.IdModelo),
                            Title = filtromodelo.Nombre,
                            Description = "Haz click aqui para ver las piezas del modelo.",
                            Data = filtromodelo,
                            Image = filtromodelo.ImageUrl
                        });
                    }
                }
                items.Add(item);
            }
        }

        private void Categorias_ItemSelected(object sender, Item e)
        {
            var simplelistpage = sender as SimpleListPage;
            simplelistpage.Navigation.PushAsync(new MainPage(e));
        }

        private void Listmarcas_ItemSelected(object sender, Item e)
        {
            if (e.Childs != null && e.Childs.Count > 0)
            {
                ObservableCollection<Item> itemsmodelos = new ObservableCollection<Item>();
                var modelos = new SimpleListPage(itemsmodelos) { Title = e.Title, RowHeight = 80 };
                foreach (var item in e.Childs)
                {
                    itemsmodelos.Add(item);
                }
                modelos.ItemSelected += Modelos_ItemSelected;
                Navigation.PushAsync(modelos);
            }
        }

        private void Modelos_ItemSelected(object sender, Item e)
        {
            var simplelistpage = sender as SimpleListPage;
            simplelistpage.Navigation.PushAsync(new MainPage(e));
        }

        private async void BtnPedidos_Clicked(object sender, EventArgs e)
        {
            await this.GoToShop();
        }

        private async void BtnPerfil_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserProfilePage(App.Oauth) { Title = "Perfil de usuario " + App.Oauth.Nombre });
        }

        private async void BtnCerrarSesion_Clicked(object sender, EventArgs e)
        {
            Settings.Data.Clear();
            await Settings.Data.SaveAsync();
            App.Instance.SetMainPage(new LoginPage());
        }

        public async void BtnContacto_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ContactPage());
        }
    }
}
