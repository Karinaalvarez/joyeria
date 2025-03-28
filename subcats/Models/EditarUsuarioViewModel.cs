using System.ComponentModel.DataAnnotations;

namespace subcats.Models
{
    public class EditarUsuarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string Username { get; set; }

        // La contraseña no es requerida para editar
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Role { get; set; } // "Admin" o "User"
    }
}
