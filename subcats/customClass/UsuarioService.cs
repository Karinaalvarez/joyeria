using Microsoft.Data.SqlClient;
using subcats.Models;
using System;
using System.Collections.Generic;

namespace subcats.customClass
{
    public class UsuarioService
    {
        private Conection _conn;

        public UsuarioService()
        {
            _conn = new Conection();
        }

        public Usuario ValidarUsuario(string username, string password)
        {
            Usuario usuario = null;
            try
            {
                _conn.connection.Open();
                string query = "SELECT Id, Username, Password, Role FROM Usuarios WHERE Username = @Username AND Password = @Password";
                _conn.sqlCommand = new SqlCommand(query, _conn.connection);
                _conn.sqlCommand.Parameters.AddWithValue("@Username", username);
                _conn.sqlCommand.Parameters.AddWithValue("@Password", password);

                _conn.sqlDataReader = _conn.sqlCommand.ExecuteReader();

                if (_conn.sqlDataReader.Read())
                {
                    usuario = new Usuario
                    {
                        Id = Convert.ToInt32(_conn.sqlDataReader["Id"]),
                        Username = _conn.sqlDataReader["Username"].ToString(),
                        Password = _conn.sqlDataReader["Password"].ToString(),
                        Role = _conn.sqlDataReader["Role"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar usuario: {ex.Message}");
            }
            finally
            {
                _conn.sqlDataReader?.Close();
                _conn.connection.Close();
            }
            return usuario;
        }

        public Usuario ObtenerUsuarioPorUsername(string username)
        {
            Usuario usuario = null;
            try
            {
                _conn.connection.Open();
                string query = "SELECT Id, Username, Password, Role FROM Usuarios WHERE Username = @Username";
                _conn.sqlCommand = new SqlCommand(query, _conn.connection);
                _conn.sqlCommand.Parameters.AddWithValue("@Username", username);

                _conn.sqlDataReader = _conn.sqlCommand.ExecuteReader();

                if (_conn.sqlDataReader.Read())
                {
                    usuario = new Usuario
                    {
                        Id = Convert.ToInt32(_conn.sqlDataReader["Id"]),
                        Username = _conn.sqlDataReader["Username"].ToString(),
                        Password = _conn.sqlDataReader["Password"].ToString(),
                        Role = _conn.sqlDataReader["Role"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario por username: {ex.Message}");
            }
            finally
            {
                _conn.sqlDataReader?.Close();
                _conn.connection.Close();
            }
            return usuario;
        }

        public List<Usuario> ObtenerTodosUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                _conn.connection.Open();
                string query = "SELECT Id, Username, Password, Role FROM Usuarios";
                _conn.sqlCommand = new SqlCommand(query, _conn.connection);

                _conn.sqlDataReader = _conn.sqlCommand.ExecuteReader();

                while (_conn.sqlDataReader.Read())
                {
                    Usuario usuario = new Usuario
                    {
                        Id = Convert.ToInt32(_conn.sqlDataReader["Id"]),
                        Username = _conn.sqlDataReader["Username"].ToString(),
                        Password = _conn.sqlDataReader["Password"].ToString(),
                        Role = _conn.sqlDataReader["Role"].ToString()
                    };
                    usuarios.Add(usuario);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
            }
            finally
            {
                _conn.sqlDataReader?.Close();
                _conn.connection.Close();
            }
            return usuarios;
        }

        public bool CrearUsuario(Usuario usuario)
        {
            bool resultado = false;
            try
            {
                _conn.connection.Open();
                string query = "INSERT INTO Usuarios (Username, Password, Role) VALUES (@Username, @Password, @Role)";
                _conn.sqlCommand = new SqlCommand(query, _conn.connection);
                _conn.sqlCommand.Parameters.AddWithValue("@Username", usuario.Username);
                _conn.sqlCommand.Parameters.AddWithValue("@Password", usuario.Password);
                _conn.sqlCommand.Parameters.AddWithValue("@Role", usuario.Role);

                int filasAfectadas = _conn.sqlCommand.ExecuteNonQuery();
                resultado = filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear usuario: {ex.Message}");
            }
            finally
            {
                _conn.connection.Close();
            }
            return resultado;
        }

        public bool ActualizarUsuario(Usuario usuario)
        {
            bool resultado = false;
            try
            {
                _conn.connection.Open();
                string query = "UPDATE Usuarios SET Username = @Username, Password = @Password, Role = @Role WHERE Id = @Id";
                _conn.sqlCommand = new SqlCommand(query, _conn.connection);
                _conn.sqlCommand.Parameters.AddWithValue("@Id", usuario.Id);
                _conn.sqlCommand.Parameters.AddWithValue("@Username", usuario.Username);
                _conn.sqlCommand.Parameters.AddWithValue("@Password", usuario.Password);
                _conn.sqlCommand.Parameters.AddWithValue("@Role", usuario.Role);

                int filasAfectadas = _conn.sqlCommand.ExecuteNonQuery();
                resultado = filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar usuario: {ex.Message}");
            }
            finally
            {
                _conn.connection.Close();
            }
            return resultado;
        }

        public bool EliminarUsuario(int id)
        {
            bool resultado = false;
            try
            {
                _conn.connection.Open();
                string query = "DELETE FROM Usuarios WHERE Id = @Id";
                _conn.sqlCommand = new SqlCommand(query, _conn.connection);
                _conn.sqlCommand.Parameters.AddWithValue("@Id", id);

                int filasAfectadas = _conn.sqlCommand.ExecuteNonQuery();
                resultado = filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar usuario: {ex.Message}");
            }
            finally
            {
                _conn.connection.Close();
            }
            return resultado;
        }
    }
} 