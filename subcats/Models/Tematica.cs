using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace subcats.Models
{
    public class Tematica
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        public string Descripcion { get; set; }

        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha de fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Activa")]
        public bool Activa { get; set; }

        // Almacena la imagen como bytes en la base de datos
        public byte[] Imagen { get; set; }

        // Propiedad para manejar la carga de archivos (no se guarda en la base de datos)
        [Display(Name = "Imagen de temática")]
        public IFormFile ImagenFile { get; set; }
    }
}
