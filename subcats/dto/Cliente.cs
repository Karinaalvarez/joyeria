using System;
using System.ComponentModel.DataAnnotations;

namespace subcats.dto
{
    public class Cliente
    {
        public int Id_cliente { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        public string Direccion { get; set; }

        public DateTime? Fecha_Creacion { get; set; }
        public DateTime? Fecha_Actualizacion { get; set; }
    }
}
