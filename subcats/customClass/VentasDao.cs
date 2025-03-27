using Microsoft.Data.SqlClient;
using subcats.dto;
using System;
using System.Collections.Generic;
using System.Data;

namespace subcats.customClass
{
    public class VentasDao
    {
        private Conection cnx;

        public VentasDao()
        {
            cnx = new Conection();
            VerificarTablasVentas();
        }

        private void VerificarTablasVentas()
        {
            try
            {
                cnx.connection.Open();
                string sql = @"
                -- Tabla para almacenar los datos del cliente y la orden
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clientes')
                BEGIN
                    CREATE TABLE Clientes (
                        Id_cliente INT PRIMARY KEY IDENTITY(1,1),
                        Nombre NVARCHAR(100) NOT NULL,
                        Apellido NVARCHAR(100) NOT NULL,
                        Email NVARCHAR(100) NOT NULL,
                        Telefono NVARCHAR(20) NOT NULL,
                        Direccion NVARCHAR(200) NOT NULL,
                        Fecha_Creacion DATETIME DEFAULT GETDATE(),
                        Fecha_Actualizacion DATETIME DEFAULT GETDATE()
                    );
                END

                -- Tabla para almacenar las órdenes
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Ordenes')
                BEGIN
                    CREATE TABLE Ordenes (
                        Id_orden INT PRIMARY KEY IDENTITY(1,1),
                        Id_cliente INT NOT NULL,
                        Fecha_Orden DATETIME DEFAULT GETDATE(),
                        Total DECIMAL(10, 2) NOT NULL,
                        Estado NVARCHAR(20) DEFAULT 'Pendiente',
                        Fecha_Creacion DATETIME DEFAULT GETDATE(),
                        Fecha_Actualizacion DATETIME DEFAULT GETDATE(),
                        FOREIGN KEY (Id_cliente) REFERENCES Clientes(Id_cliente)
                    );
                END

                -- Tabla para almacenar los detalles de la orden
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DetallesOrden')
                BEGIN
                    CREATE TABLE DetallesOrden (
                        Id_detalle INT PRIMARY KEY IDENTITY(1,1),
                        Id_orden INT NOT NULL,
                        Id_producto INT NOT NULL,
                        Cantidad INT NOT NULL,
                        Precio_Unitario DECIMAL(10, 2) NOT NULL,
                        Subtotal DECIMAL(10, 2) NOT NULL,
                        Fecha_Creacion DATETIME DEFAULT GETDATE(),
                        Fecha_Actualizacion DATETIME DEFAULT GETDATE(),
                        FOREIGN KEY (Id_orden) REFERENCES Ordenes(Id_orden),
                        FOREIGN KEY (Id_producto) REFERENCES productos(id_producto)
                    );
                END

                -- Trigger para actualizar la fecha de actualización en Clientes
                IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Clientes_Update')
                BEGIN
                    EXEC('
                    CREATE TRIGGER TR_Clientes_Update
                    ON Clientes
                    AFTER UPDATE
                    AS
                    BEGIN
                        UPDATE Clientes
                        SET Fecha_Actualizacion = GETDATE()
                        FROM Clientes c
                        INNER JOIN inserted i ON c.Id_cliente = i.Id_cliente;
                    END
                    ');
                END

                -- Trigger para actualizar la fecha de actualización en Ordenes
                IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Ordenes_Update')
                BEGIN
                    EXEC('
                    CREATE TRIGGER TR_Ordenes_Update
                    ON Ordenes
                    AFTER UPDATE
                    AS
                    BEGIN
                        UPDATE Ordenes
                        SET Fecha_Actualizacion = GETDATE()
                        FROM Ordenes o
                        INNER JOIN inserted i ON o.Id_orden = i.Id_orden;
                    END
                    ');
                END

                -- Trigger para actualizar la fecha de actualización en DetallesOrden
                IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_DetallesOrden_Update')
                BEGIN
                    EXEC('
                    CREATE TRIGGER TR_DetallesOrden_Update
                    ON DetallesOrden
                    AFTER UPDATE
                    AS
                    BEGIN
                        UPDATE DetallesOrden
                        SET Fecha_Actualizacion = GETDATE()
                        FROM DetallesOrden d
                        INNER JOIN inserted i ON d.Id_detalle = i.Id_detalle;
                    END
                    ');
                END

                -- Trigger para actualizar el stock de productos después de una orden
                IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_DetallesOrden_Insert')
                BEGIN
                    EXEC('
                    CREATE TRIGGER TR_DetallesOrden_Insert
                    ON DetallesOrden
                    AFTER INSERT
                    AS
                    BEGIN
                        UPDATE productos
                        SET stock = p.stock - i.Cantidad,
                            fecha_actualizacion = GETDATE()
                        FROM productos p
                        INNER JOIN inserted i ON p.id_producto = i.Id_producto;
                    END
                    ');
                END
                ";

                SqlCommand cmd = new SqlCommand(sql, cnx.connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al verificar tablas de ventas: " + ex.Message);
            }
            finally
            {
                cnx.connection.Close();
            }
        }

        // Método para insertar un cliente y devolver su ID
        public int InsertarCliente(Cliente cliente)
        {
            int clienteId = 0;
            try
            {
                cnx.connection.Open();
                string sql = @"
                INSERT INTO Clientes (Nombre, Apellido, Email, Telefono, Direccion)
                VALUES (@Nombre, @Apellido, @Email, @Telefono, @Direccion);
                SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(sql, cnx.connection);
                cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                cmd.Parameters.AddWithValue("@Email", cliente.Email);
                cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion);

                // Ejecutar y obtener el ID generado
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    clienteId = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar cliente: " + ex.Message);
            }
            finally
            {
                cnx.connection.Close();
            }
            return clienteId;
        }

        // Método para insertar una orden y devolver su ID
        public int InsertarOrden(Orden orden)
        {
            int ordenId = 0;
            try
            {
                cnx.connection.Open();
                string sql = @"
                INSERT INTO Ordenes (Id_cliente, Total, Estado)
                VALUES (@Id_cliente, @Total, @Estado);
                SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(sql, cnx.connection);
                cmd.Parameters.AddWithValue("@Id_cliente", orden.Id_cliente);
                cmd.Parameters.AddWithValue("@Total", orden.Total);
                cmd.Parameters.AddWithValue("@Estado", orden.Estado);

                // Ejecutar y obtener el ID generado
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    ordenId = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar orden: " + ex.Message);
            }
            finally
            {
                cnx.connection.Close();
            }
            return ordenId;
        }

        // Método para insertar los detalles de una orden
        public bool InsertarDetallesOrden(List<DetalleOrden> detalles)
        {
            bool exito = false;
            try
            {
                cnx.connection.Open();
                SqlTransaction transaction = cnx.connection.BeginTransaction();

                try
                {
                    foreach (var detalle in detalles)
                    {
                        string sql = @"
                        INSERT INTO DetallesOrden (Id_orden, Id_producto, Cantidad, Precio_Unitario, Subtotal)
                        VALUES (@Id_orden, @Id_producto, @Cantidad, @Precio_Unitario, @Subtotal);";

                        SqlCommand cmd = new SqlCommand(sql, cnx.connection, transaction);
                        cmd.Parameters.AddWithValue("@Id_orden", detalle.Id_orden);
                        cmd.Parameters.AddWithValue("@Id_producto", detalle.Id_producto);
                        cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                        cmd.Parameters.AddWithValue("@Precio_Unitario", detalle.Precio_Unitario);
                        cmd.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    exito = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error en la transacción: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar detalles de orden: " + ex.Message);
            }
            finally
            {
                cnx.connection.Close();
            }
            return exito;
        }

        // Método para obtener una orden completa con sus detalles
        public Orden ObtenerOrdenCompleta(int ordenId)
        {
            Orden orden = null;
            try
            {
                cnx.connection.Open();
                
                // Obtener la orden
                string sqlOrden = @"
                SELECT o.*, c.Nombre, c.Apellido, c.Email, c.Telefono, c.Direccion
                FROM Ordenes o
                INNER JOIN Clientes c ON o.Id_cliente = c.Id_cliente
                WHERE o.Id_orden = @Id_orden";

                SqlCommand cmdOrden = new SqlCommand(sqlOrden, cnx.connection);
                cmdOrden.Parameters.AddWithValue("@Id_orden", ordenId);

                using (SqlDataReader reader = cmdOrden.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        orden = new Orden
                        {
                            Id_orden = Convert.ToInt32(reader["Id_orden"]),
                            Id_cliente = Convert.ToInt32(reader["Id_cliente"]),
                            Fecha_Orden = Convert.ToDateTime(reader["Fecha_Orden"]),
                            Total = Convert.ToDecimal(reader["Total"]),
                            Estado = reader["Estado"].ToString(),
                            Cliente = new Cliente
                            {
                                Id_cliente = Convert.ToInt32(reader["Id_cliente"]),
                                Nombre = reader["Nombre"].ToString(),
                                Apellido = reader["Apellido"].ToString(),
                                Email = reader["Email"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Direccion = reader["Direccion"].ToString()
                            }
                        };
                    }
                }

                if (orden != null)
                {
                    // Obtener los detalles de la orden
                    string sqlDetalles = @"
                    SELECT d.*, p.nombre as ProductoNombre, p.descripcion as ProductoDescripcion
                    FROM DetallesOrden d
                    INNER JOIN productos p ON d.Id_producto = p.id_producto
                    WHERE d.Id_orden = @Id_orden";

                    SqlCommand cmdDetalles = new SqlCommand(sqlDetalles, cnx.connection);
                    cmdDetalles.Parameters.AddWithValue("@Id_orden", ordenId);

                    using (SqlDataReader reader = cmdDetalles.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DetalleOrden detalle = new DetalleOrden
                            {
                                Id_detalle = Convert.ToInt32(reader["Id_detalle"]),
                                Id_orden = Convert.ToInt32(reader["Id_orden"]),
                                Id_producto = Convert.ToInt32(reader["Id_producto"]),
                                Cantidad = Convert.ToInt32(reader["Cantidad"]),
                                Precio_Unitario = Convert.ToDecimal(reader["Precio_Unitario"]),
                                Subtotal = Convert.ToDecimal(reader["Subtotal"]),
                                Producto = new Producto
                                {
                                    Id_producto = Convert.ToInt32(reader["Id_producto"]),
                                    Nombre = reader["ProductoNombre"].ToString(),
                                    Descripcion = reader["ProductoDescripcion"].ToString()
                                }
                            };
                            orden.Detalles.Add(detalle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener orden completa: " + ex.Message);
            }
            finally
            {
                cnx.connection.Close();
            }
            return orden;
        }
    }
}
