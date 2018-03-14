using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShoppingCart
{
    public static class Carrito
    {

        public static async Task GoToShop(this Page page)
        {
            if (App.ShopList != null && App.ShopList.Count > 0)
            {
                await page.Navigation.PushAsync(new CarPage());
            }
        }

    }
}
