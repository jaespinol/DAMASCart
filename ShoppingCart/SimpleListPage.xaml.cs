using DevAzt.FormsX.Storage.Isolated;
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
    public partial class SimpleListPage : ContentPage
    {
        private ObservableCollection<Item> _items; 
        public ObservableCollection<Item> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                ListItems.ItemsSource = _items;
            }
        }
        
        public int RowHeight
        {
            set
            {
                ListItems.RowHeight = value;
            }
        }

        public event EventHandler<Item> ItemSelected;

        protected virtual void OnItemSelected(Item item)
        {
            if (ItemSelected != null)
            {
                ItemSelected.Invoke(this, item);
            }
        }

        public SimpleListPage(ObservableCollection<Item> items)
        {
            InitializeComponent();
            Items = items;
            ListItems.ItemsSource = Items;
        }
        
        private void ListItems_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ListItems.SelectedItem = null;
        }

        private void ListItems_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (ListItems.SelectedItem != null)
            {
                var item = ListItems.SelectedItem as Item;
                OnItemSelected(item);
            }
        }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }
        public ObservableCollection<Item> Childs { get; set; }
        public string Image { get; set; }


    }
}
