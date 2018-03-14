using ShoppingCart.API.Result;

namespace ShoppingCart
{
    public class OauthResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public int IdUsuario { get; set; }
        public int IdPerfil { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public int HasDirection { get; set; }
        public string Alias { get; set; }
        public string Password { get; set; }
        public string Activo { get; set; }
        public string Eliminado { get; set; }
        public string Fecha { get; set; }
        public string IdPerfi { get; set; }
        public int RedTecnicos { get; set; }
        public Direction Direction { get; set; }
        public string CustomerId { get; set; }
    }
}