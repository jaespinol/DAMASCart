using ShoppingCart.API;
using ShoppingCart.API.Result;
using ShoppingCart.ViewModel;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShoppingCart
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExcelPage : ContentPage
    {
        public ExcelPage()
        {
            InitializeComponent();
        }

        private async void OpenFile_Clicked(object sender, EventArgs e)
        {
            var filedata = await CrossFilePicker.Current.PickFile();
            var stream = new MemoryStream(filedata.DataArray);
            StreamReader file = new StreamReader(stream);
            string line = "";
            List<string> lines = new List<string>();
            while ((line = file.ReadLine()) != null)
            {
                line = line.Replace('"', ' ');
                lines.Add(line);
                System.Diagnostics.Debug.WriteLine(line);
            }
            file.Dispose();
            // var cvsstring = System.Text.Encoding.UTF8.GetString(filedata.DataArray, 0, filedata.DataArray.Length);
            // cvsstring = cvsstring.Replace('"', ' ');
            UploadCVS cvs = new UploadCVS { Lines = lines };
            try
            {
                cvs.MakeTableProduct();
                await cvs.UploadProduct(AddProduct);
                await DisplayAlert(App.AppName, "Se subierón los productos", "Aceptar");
            }
            catch
            {

            }
        }

        public async Task<bool> AddProduct(Dictionary<string, object> productinsert, Dictionary<string, object> priceinsert)
        {
            var result = await App.RestClient.Post<StandarResult>(App.BaseUrl + "/Product/Add", simpleparams: productinsert);
            if (result != null)
            {
                if (result.InsertId > 0)
                {
                    priceinsert.Add("IdUsuario", "3");
                    priceinsert.Add("IdProducto", result.InsertId.ToString());
                    var priceresult = await App.RestClient.Post<StandarResult>(App.BaseUrl + "/Price/Add", priceinsert);
                    if (priceresult != null)
                    {
                        if (priceresult.InsertId == -1)
                        {
                            await DisplayAlert(App.AppName, $"No pudimos guardar el precio del producto: {productinsert["nombre"]}", "Aceptar");
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    await DisplayAlert(App.AppName, $"No pudimos guardar el producto: {productinsert["nombre"]}", "Aceptar");
                    return false;
                }
            }
            return false;
        }

        private async void OpenStock_Clicked(object sender, EventArgs e)
        {
            var filedata = await CrossFilePicker.Current.PickFile();
            var stream = new MemoryStream(filedata.DataArray);
            StreamReader file = new StreamReader(stream);
            string line = "";
            List<string> lines = new List<string>();
            while ((line = file.ReadLine()) != null)
            {
                line = line.Replace('"', ' ');
                lines.Add(line);
                System.Diagnostics.Debug.WriteLine(line);
            }
            file.Dispose();
            UploadCVS cvs = new UploadCVS { Lines = lines };
            try
            {
                cvs.MakeTableStock();
                if(await cvs.UploadStock(StockUpload))
                {
                    await DisplayAlert(App.AppName, "Se subio el stock", "Aceptar");
                }
                else
                {
                    await DisplayAlert(App.AppName, "Ocurrió un error al subir stock", "Aceptar");
                }
            }
            catch
            {

            }
        }

        private async Task<bool> StockUpload(List<Stock> stocklist)
        {   
            StockRequest request = new StockRequest { IdUsuario = "3", StockList = stocklist };
            StandarResult result = await App.RestClient.Post<StandarResult, StockRequest>(App.BaseUrl + "/Stock/AddList", request);
            if (result != null) return result.Code == 100;
            return false;
        }
    }
}
