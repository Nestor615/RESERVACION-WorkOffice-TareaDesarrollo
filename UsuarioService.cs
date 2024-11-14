using MySql.Data.MySqlClient;
using RESERVACION_WorkOffice_TareaDesarrollo.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESERVACION_WorkOffice_TareaDesarrollo
{
    public class UsuarioService
    {
        private string connectionString = "Server=localhost;Database=reservacion;User ID=root;"; // Configura tu cadena de conexión aquí

        // Método para crear un nuevo usuario en la base de datos
        public void CrearUsuario(Usuario usuario)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO usuarios (nombreUsuario, rol, contrasena) VALUES (@Nombre, @Rol, @Contraseña)";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
                    cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña); // Asegúrate de encriptar las contraseñas

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Método para obtener la lista de todos los usuarios
        public List<Usuario> ObtenerUsuarios()
        {
            List<Usuario> listaUsuarios = new List<Usuario>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT idUsuario, nombreUsuario, rol FROM usuarios";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Usuario usuario = new Usuario(
                                reader.GetInt32("idUsuario"),
                                reader.GetString("nombreUsuario"),
                                reader.GetString("rol"),
                                null
                            );
                            listaUsuarios.Add(usuario);
                        }
                    }
                }
            }
            return listaUsuarios;
        }
    }

}
