using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.ORM
{
    public class Usuario
    {
        [PrimaryKey]
        public int IdUsuario { get; set; }
        public string Alias { get; set; }
        public int IdPerfil { get; internal set; }
    }
}
