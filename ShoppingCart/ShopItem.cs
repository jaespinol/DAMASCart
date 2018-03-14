using Xamarin.Forms;

namespace ShoppingCart
{
    public class ShopItem
    {

        public int Qty { get; set; }
        public int IdProduct { get; internal set; }
        public double Price { get; internal set; }
        public Producto Product { get; internal set; }
        public Entry BoxEdit { get; internal set; }
        public Button BtnEdit { get; internal set; }
        public Button BtnCancel { get; internal set; }

        public override bool Equals(object obj)
        {
            var item = obj as ShopItem;
            if(item != null)
            {
                return item.IdProduct == this.IdProduct;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return IdProduct;
        }
    }
}