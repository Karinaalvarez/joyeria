using System;
using System.ComponentModel.DataAnnotations;

namespace subcats.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
        [Display(Name = "Correo Electrónico")]
        public string Username { get; set; }

        // Se ha eliminado el atributo Required para permitir editar usuarios sin cambiar la contraseña
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Role { get; set; } // "Admin" o "User"
    }
} 