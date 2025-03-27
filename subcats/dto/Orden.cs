using System;
using System.Collections.Generic;

namespace subcats.dto
{
    public class Orden
    {
        public int Id_orden { get; set; }
        public int Id_cliente { get; set; }
        public DateTime Fecha_Orden { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public DateTime? Fecha_Creacion { get; set; }
        public DateTime? Fecha_Actualizacion { get; set; }
        
        // Propiedades de navegación
        public Cliente Cliente { get; set; }
        public List<DetalleOrden> Detalles { get; set; }
        
        // Propiedades para reportes
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        
        public Orden()
        {
            Fecha_Orden = DateTime.Now;
            Estado = "Pendiente";
            Detalles = new List<DetalleOrden>();
        }
    }
}
