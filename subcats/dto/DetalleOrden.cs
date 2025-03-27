using System;

namespace subcats.dto
{
    public class DetalleOrden
    {
        public int Id_detalle { get; set; }
        public int Id_orden { get; set; }
        public int Id_producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio_Unitario { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime? Fecha_Creacion { get; set; }
        public DateTime? Fecha_Actualizacion { get; set; }
        
        // Propiedades de navegación
        public Producto Producto { get; set; }
    }
}
