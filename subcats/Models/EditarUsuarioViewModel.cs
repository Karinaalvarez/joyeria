using System.ComponentModel.DataAnnotations;

namespace subcats.Models
{
    public class EditarUsuarioViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
        [Display(Name = "Correo Electrónico")]
        public string Username { get; set; }

        // La contraseña no es requerida para editar
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Role { get; set; } // "Admin" o "User"
    }
}
