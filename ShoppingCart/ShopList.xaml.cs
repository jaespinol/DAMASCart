using ShoppingCart.API;
using ShoppingCart.API.Result;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShoppingCart
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShopList : ContentPage
    {
        public ShopList(List<API.Result.Producto> productos)
        {
            InitializeComponent();

            if (productos != null)
            {
                var items = new List<API.Result.Producto>();
                foreach (var producto in productos)
                {
                    for (int i = 0; i < int.Parse(producto.cantidad); i++)
                    {
                        var item = new API.Result.Producto
                        {
                            cantidad = (i + 1).ToString(),
                            costo = producto.costo,
                            nombre = producto.nombre,
                            parte = producto.parte
                        };
                        
                        if (producto.Calificacion != null && producto.Calificacion.Count > 0)
                        {
                            if(producto.Calificacion.Count <= int.Parse(producto.cantidad))
                            {
                                item.Calificacion = new List<Calificacion>
                                {
                                    producto.Calificacion[i]
                                };
                            }
                        }
                        items.Add(item);
                    }
                }
                ListProducts.ItemsSource = items;
            }
        }

        private void ListProducts_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as API.Result.Producto;
            if (item == null) return;
            /*
            if (item.Calificacion != null && item.Calificacion.Count > 0)
            {
                await Navigation.PushAsync(new ShopItemDetail(item.Calificacion[0]));
            }
            */
        }

        private void ListProducts_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListProducts.SelectedItem = null;
        }
    }
}
