using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Data.SqlClient;
using subcats.Models;

namespace subcats.customClass
{
    public class TematicaService
    {
        private readonly string _connectionString;
        private readonly Conection _cnx;

        public TematicaService()
        {
            _cnx = new Conection();
            VerificarTablaTematicas();
        }

        // Método para verificar y crear la tabla de temáticas si no existe
        private void VerificarTablaTematicas()
        {
            try
            {
                _cnx.connection.Open();
                string query = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tematica')
                BEGIN
                    CREATE TABLE tematica (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Nombre NVARCHAR(100) NOT NULL,
                        Descripcion NVARCHAR(500) NULL,
                        FechaInicio DATE NOT NULL,
                        FechaFin DATE NOT NULL,
                        Activa BIT NOT NULL DEFAULT 0,
                        Imagen VARBINARY(MAX) NULL
                    );
                END";
                
                using (SqlCommand command = new SqlCommand(query, _cnx.connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar tabla de temáticas: {ex.Message}");
                throw;
            }
            finally
            {
                _cnx.connection.Close();
            }
        }

        // Obtener todas las temáticas
        public List<Tematica> ObtenerTematicas()
        {
            List<Tematica> tematicas = new List<Tematica>();

            try
            {
                _cnx.connection.Open();
                string query = "SELECT Id, Nombre, Descripcion, FechaInicio, FechaFin, Activa FROM tematica";
                
                using (SqlCommand command = new SqlCommand(query, _cnx.connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tematica tematica = new Tematica
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : null,
                                FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                                FechaFin = Convert.ToDateTime(reader["FechaFin"]),
                                Activa = Convert.ToBoolean(reader["Activa"])
                            };
                            
                            tematicas.Add(tematica);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener temáticas: {ex.Message}");
                throw;
            }
            finally
            {
                _cnx.connection.Close();
            }

            return tematicas;
        }

        // Obtener una temática por su ID
        public Tematica ObtenerTematica(int id)
        {
            Tematica tematica = null;

            try
            {
                _cnx.connection.Open();
                string query = "SELECT Id, Nombre, Descripcion, FechaInicio, FechaFin, Activa, Imagen FROM tematica WHERE Id = @Id";
                
                using (SqlCommand command = new SqlCommand(query, _cnx.connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tematica = new Tematica
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : null,
                                FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                                FechaFin = Convert.ToDateTime(reader["FechaFin"]),
                                Activa = Convert.ToBoolean(reader["Activa"])
                            };

                            // Obtener la imagen si existe
                            if (reader["Imagen"] != DBNull.Value)
                            {
                                tematica.Imagen = (byte[])reader["Imagen"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener temática: {ex.Message}");
                throw;
            }
            finally
            {
                _cnx.connection.Close();
            }

            return tematica;
        }

        // Obtener la temática activa actual
        public Tematica ObtenerTematicaActiva()
        {
            Tematica tematica = null;
            DateTime fechaActual = DateTime.Now.Date;

            try
            {
                _cnx.connection.Open();
                string query = @"SELECT Id, Nombre, Descripcion, FechaInicio, FechaFin, Activa, Imagen 
                                FROM tematica 
                                WHERE Activa = 1 AND @FechaActual BETWEEN FechaInicio AND FechaFin 
                                ORDER BY FechaInicio DESC";
                
                using (SqlCommand command = new SqlCommand(query, _cnx.connection))
                {
                    command.Parameters.AddWithValue("@FechaActual", fechaActual);
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tematica = new Tematica
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : null,
                                FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                                FechaFin = Convert.ToDateTime(reader["FechaFin"]),
                                Activa = Convert.ToBoolean(reader["Activa"])
                            };

                            // Obtener la imagen si existe
                            if (reader["Imagen"] != DBNull.Value)
                            {
                                tematica.Imagen = (byte[])reader["Imagen"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener temática activa: {ex.Message}");
                throw;
            }
            finally
            {
                _cnx.connection.Close();
            }

            return tematica;
        }

        // Crear una nueva temática
        public int CrearTematica(Tematica tematica)
        {
            int id = 0;

            try
            {
                _cnx.connection.Open();
                string query = @"INSERT INTO tematica (Nombre, Descripcion, FechaInicio, FechaFin, Activa, Imagen) 
                                VALUES (@Nombre, @Descripcion, @FechaInicio, @FechaFin, @Activa, @Imagen);
                                SELECT SCOPE_IDENTITY();";
                
                using (SqlCommand command = new SqlCommand(query, _cnx.connection))
                {
                    command.Parameters.AddWithValue("@Nombre", tematica.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", tematica.Descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaInicio", tematica.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", tematica.FechaFin);
                    command.Parameters.AddWithValue("@Activa", tematica.Activa);
                    command.Parameters.AddWithValue("@Imagen", tematica.Imagen ?? (object)DBNull.Value);
                    
                    var result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        id = Convert.ToInt32(result);
                    }
                }

                // Si esta temática está activa, desactivar las demás
                if (tematica.Activa)
                {
                    DesactivarOtrasTematicas(id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear temática: {ex.Message}");
                throw;
            }
            finally
            {
                _cnx.connection.Close();
            }

            return id;
        }

        // Actualizar una temática existente
        public bool ActualizarTematica(Tematica tematica)
        {
            bool resultado = false;

            try
            {
                _cnx.connection.Open();
                
                // Si la imagen no ha cambiado, no la actualizamos
                string query;
                if (tematica.Imagen == null)
                {
                    query = @"UPDATE tematica 
                            SET Nombre = @Nombre, 
                                Descripcion = @Descripcion, 
                                FechaInicio = @FechaInicio, 
                                FechaFin = @FechaFin, 
                                Activa = @Activa 
                            WHERE Id = @Id";
                }
                else
                {
                    query = @"UPDATE tematica 
                            SET Nombre = @Nombre, 
                                Descripcion = @Descripcion, 
                                FechaInicio = @FechaInicio, 
                                FechaFin = @FechaFin, 
                                Activa = @Activa, 
                                Imagen = @Imagen 
                            WHERE Id = @Id";
                }
                
                using (SqlCommand command = new SqlCommand(query, _cnx.connection))
                {
                    command.Parameters.AddWithValue("@Id", tematica.Id);
                    command.Parameters.AddWithValue("@Nombre", tematica.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", tematica.Descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaInicio", tematica.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", tematica.FechaFin);
                    command.Parameters.AddWithValue("@Activa", tematica.Activa);
                    
                    if (tematica.Imagen != null)
                    {
                        command.Parameters.AddWithValue("@Imagen", tematica.Imagen);
                    }
                    
                    int filasAfectadas = command.ExecuteNonQuery();
                    resultado = filasAfectadas > 0;
                }

                // Si esta temática está activa, desactivar las demás
                if (tematica.Activa)
                {
                    DesactivarOtrasTematicas(tematica.Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar temática: {ex.Message}");
                throw;
            }
            finally
            {
                _cnx.connection.Close();
            }

            return resultado;
        }

        // Eliminar una temática
        public bool EliminarTematica(int id)
        {
            bool resultado = false;

            try
            {
                _cnx.connection.Open();
                string query = "DELETE FROM tematica WHERE Id = @Id";
                
                using (SqlCommand command = new SqlCommand(query, _cnx.connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    
                    int filasAfectadas = command.ExecuteNonQuery();
                    resultado = filasAfectadas > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar temática: {ex.Message}");
                throw;
            }
            finally
            {
                _cnx.connection.Close();
            }

            return resultado;
        }

        // Método auxiliar para desactivar otras temáticas cuando se activa una
        private void DesactivarOtrasTematicas(int idActiva)
        {
            try
            {
                string query = "UPDATE tematica SET Activa = 0 WHERE Id != @IdActiva";
                
                using (SqlCommand command = new SqlCommand(query, _cnx.connection))
                {
                    command.Parameters.AddWithValue("@IdActiva", idActiva);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al desactivar otras temáticas: {ex.Message}");
                throw;
            }
        }
    }
}
