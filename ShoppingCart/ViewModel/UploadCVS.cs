
using ShoppingCart.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShoppingCart.ViewModel
{
    public class UploadCVS
    {
        public Table Table { get; set; }
        public List<string> Lines { get; internal set; }

        public UploadCVS()
        {
            Table = new Table();
        }

        public async Task UploadProduct(Func<Dictionary<string, object>, Dictionary<string, object>, Task<bool>> lamda)
        {
            Table.RemoveColumn(Head.Name("end"));
            var tabletemp = new Table();
            var productinsert = new Dictionary<string, object>();
            var priceinsert = new Dictionary<string, object>();
            foreach (var row in Table.Rows)
            {
                System.Diagnostics.Debug.WriteLine(row.ToString());
                if (row.ToString() != string.Empty)
                {
                    if (row.ToString().Contains("REF"))
                    {
                        break;
                    }
                    foreach (var column in row)
                    {
                        if (column.ColumnName.Equals("preciomayoreo") || column.ColumnName.Equals("preciounitario") || column.ColumnName.Equals("cantidadmayoreo") || column.ColumnName.Equals("cantidadunitario"))
                        {
                            priceinsert.Add(column.ColumnName, column.ValueOfColumn.ToUpper());
                        }
                        else
                        {
                            productinsert.Add(column.ColumnName, column.ValueOfColumn.ToUpper());
                        }
                    }

                    if (!(await lamda.Invoke(productinsert, priceinsert)))
                    {
                        foreach (var column in row)
                        {
                            tabletemp.Add(column);
                        }
                    }
                    priceinsert = new Dictionary<string, object>();
                    productinsert = new Dictionary<string, object>();
                }
            }
        }

        public void MakeTableStock()
        {
            var rows = Lines;
            rows.RemoveAt(0);
            rows.RemoveAll(e => string.IsNullOrEmpty(e));

            foreach (var rowdefinition in rows)
            {
                if (!string.IsNullOrEmpty(rowdefinition))
                {
                    var row = rowdefinition.Split(',').ToList();
                    int i = 0;
                    foreach (var item in row)
                    {
                        string columname = "codigo";
                        switch (i)
                        {
                            case 0:
                                columname = "codigo";
                                break;

                            case 1:
                                columname = "qty";
                                break;

                            case 2:
                                columname = "end";
                                break;
                        }

                        string column = " ";
                        try
                        {
                            column = Regex.Replace(item, @"\t|\n|\r", "");
                        }
                        catch
                        {

                        }
                        Table.Add(new Column { ColumnName = columname, ValueOfColumn = column });
                        i++;
                    }
                }
            }
        }

        public async Task<bool> UploadStock(Func<List<Stock>, Task<bool>> addProduct)
        {
            List<Stock> stocklist = new List<Stock>();
            foreach (var row in Table.Rows)
            {
                Dictionary<string, string> stocks = new Dictionary<string, string>();
                foreach (var column in row)
                {
                    stocks.Add(column.ColumnName, column.ValueOfColumn);
                }
                stocks.Add("idusuario", "3");
                if (stocks.ContainsKey("codigo") && stocks.ContainsKey("qty") && stocks.ContainsKey("idusuario"))
                {
                    Stock stock = new Stock { Nombre = stocks["codigo"], Qty = stocks["qty"], IdUsuario = stocks["idusuario"] };
                    stocklist.Add(stock);
                }
                else
                {
                    throw new Exception("No esta completa la clase");
                }
            }

            return await addProduct.Invoke(stocklist);
        }

        public void MakeTableProduct()
        {
            var rows = Lines;
            rows.RemoveAt(0);
            rows.RemoveAll(e => string.IsNullOrEmpty(e));
            foreach (var rowdefinition in rows)
            {
                int i = 0;
                if (!string.IsNullOrEmpty(rowdefinition))
                {
                    var row = Regex.Replace(rowdefinition, @"\t|\n|\r", "").Split(',').ToList();
                    foreach (var item in row)
                    {
                        string columname = "nombre";
                        switch (i)
                        {
                            case 0:
                                columname = "nombre";
                                break;

                            case 1:
                                columname = "parte";
                                break;

                            case 2:
                                columname = "color";
                                break;

                            case 3:
                                columname = "preciomayoreo";
                                break;

                            case 4:
                                columname = "preciounitario";
                                break;

                            case 5:
                                columname = "tags";
                                break;

                            case 6:
                                columname = "cantidadmayoreo";
                                break;

                            case 7:
                                columname = "cantidadunitario";
                                break;

                            case 8:
                                columname = "end";
                                break;
                        }

                        string column = " ";
                        try
                        {
                            column = Regex.Replace(item, @"\t|\n|\r", "");
                            if (i == 7 && string.IsNullOrEmpty(column))
                            {
                                column = "1";
                            }
                        }
                        catch
                        {

                        }
                        Table.Add(new Column { ColumnName = columname, ValueOfColumn = column });
                        i++;
                    }
                }
            }
        }
    }
}
