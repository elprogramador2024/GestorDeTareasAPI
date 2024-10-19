namespace GestorDeTareas.Models
{
    public class Usuario
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public Rol Rol { get; set; }
    }

    public class RegisterUser : Usuario
    {
        public string Password { get; set; }
    }
}
