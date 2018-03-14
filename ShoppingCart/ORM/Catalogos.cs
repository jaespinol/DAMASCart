using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.ORM
{
    public class Marca
    {
        [PrimaryKey]
        public string IdMarca { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }

        public string ImageUrl
        {
            get
            {
                return string.Format("{0}/uploads/{1}", App.BaseUrl, Imagen);
            }
        }
    }

    public class Modelo
    {
        [PrimaryKey]
        public string IdModelo { get; set; }
        public string Nombre { get; set; }
        public string IdMarca { get; set; }
        public string Imagen { get; set; }

        public string ImageUrl
        {
            get
            {
                return string.Format("{0}/uploads/{1}", App.BaseUrl, Imagen);
            }
        }
    }
    
    public class Categoria
    {
        [PrimaryKey]
        public string IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }

        public string ImageUrl
        {
            get
            {
                return string.Format("{0}/uploads/{1}", App.BaseUrl, Imagen);
            }
        }
    }

    public class Catalogos
    {
        public List<Marca> Marcas { get; set; }
        public List<Modelo> Modelos { get; set; }
        public List<Categoria> Categorias { get; set; }
    }
}
