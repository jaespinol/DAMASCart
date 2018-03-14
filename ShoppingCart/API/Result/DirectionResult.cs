using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.API.Result
{
    public class Direction
    {
        public string IdDireccion { get; set; }
        public string Dnicuit { get; set; }
        public string Empresa { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string CodigoPostal { get; set; }
        public string Provincia { get; set; }
        public string Fecha { get; set; }
        public string GPS { get; set; }
        public string IdUsuario { get; set; }

        public override string ToString()
        {
            var response = 
                "Direccion: " + Direccion + "\n" +
                "Provincia: " + Provincia + "\n" + 
                "Ciudad: " + Ciudad + "\n" +
                "Codigo Postal: " + CodigoPostal + "\n" +
                "DNI/CUIT: " + Dnicuit + "\n" +
                "Empresa: " + Empresa + "\n";
            return response;
        }
    }

    public class DirectionResult
    {
        public string Message { get; set; }
        public int Code { get; set; }
        public string InsertId { get; set; }
        public Direction Direccion { get; set; }
    }
}
