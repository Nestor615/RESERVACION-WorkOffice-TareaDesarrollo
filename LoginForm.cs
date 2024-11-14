using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RESERVACION_WorkOffice_TareaDesarrollo
{
    public partial class LoginForm : Form
    {
        public string connectionString = "Server=localhost;Database=reservacion;User ID=root;";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txtUsuario.Text;
            string contrasena = txtContrasena.Text;

            if (VerificarCredenciales(nombreUsuario, contrasena))
            {
                MessageBox.Show("Inicio de sesión exitoso", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Abre la ventana principal de la aplicación o el menú
                Inicio mainForm = new Inicio();
                mainForm.Show();
                this.Hide(); // Oculta el formulario de login
            }
            else
            {
                MessageBox.Show("Nombre de usuario o contraseña incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }         
        }

        private bool VerificarCredenciales(string nombreUsuario, string contrasena)
        {
            bool isAuthenticated = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(1) FROM Usuarios WHERE nombreUsuario = @nombreUsuario AND contrasena = @contrasena";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    command.Parameters.AddWithValue("@contrasena", contrasena);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    isAuthenticated = (count == 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectarse a la base de datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isAuthenticated;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
