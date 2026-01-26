namespace Quiniela.Models
{
    public class RegisterRequest
    {
        required
        public string UserName { get; set; }

        required
        public string Nombres { get; set; }

        required
        public string Email { get; set; }

        required
        public string Password { get; set; }

        required
        public string CodigoReferencia { get; set; }
    }
}