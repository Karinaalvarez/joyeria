using System;
using subcats.customClass;

namespace subcats
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando proceso para agregar la columna NumeroIdentidad a la tabla Clientes...");
            
            try
            {
                VentasDao ventasDao = new VentasDao();
                ventasDao.CrearTablas();
                
                Console.WriteLine("Proceso completado con éxito.");
                Console.WriteLine("La columna NumeroIdentidad ha sido agregada a la tabla Clientes si no existía previamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
