

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShoppingCart.ORM;
using Xamarin.Forms;

namespace DevAzt.FormsX.Storage.SQLite.LiteConnection
{
    public class LocalDB : DataBase
    {
        public Table<Usuario> Usuario { get; set; }
        public Table<Modelo> Modelo { get; set; }
        public Table<Categoria> Categoria { get; set; }
        public Table<Marca> Marca { get; set; }
        
        public LocalDB(string databasePath, bool storeDateTimeAsTicks = true) : base(databasePath, storeDateTimeAsTicks)
        {
            Usuario = DBSet<Usuario>();
            Modelo = DBSet<Modelo>();
            Categoria = DBSet<Categoria>();
            Marca = DBSet<Marca>();
        }
        
        public static LocalDB Instance
        {
            get
            {
                var idatabase = DependencyService.Get<IDataBase>();
                if (idatabase == null) throw new NullReferenceException("La dependencia de servicio es null");
                return idatabase.GetDataBase();
            }
        }
    }
}
