using RESERVACION_WorkOffice_TareaDesarrollo.Clases;
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
    public partial class FormUsuarios : Form
    {
        private UsuarioService usuarioService = new UsuarioService();

        public FormUsuarios()
        {
            InitializeComponent();
            CargarUsuarios(); // Cargar la lista de usuarios al iniciar el formulario
        }

        private void CargarUsuarios()
        {
            List<Usuario> usuarios = usuarioService.ObtenerUsuarios();
            dgvUsuarios.AutoGenerateColumns = true;
            dgvUsuarios.DataSource = usuarios;
        }


        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            // Validar datos de entrada
            if (string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(cmbRoles.Text) || string.IsNullOrEmpty(txtContraseña.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Crear un nuevo usuario con los datos del formulario
            Usuario nuevoUsuario = new Usuario(
                0, // El ID se genera en la base de datos
                txtNombre.Text,
                cmbRoles.Text,
                txtContraseña.Text
            );

            // Llamar al servicio para guardar el usuario
            usuarioService.CrearUsuario(nuevoUsuario);

            // Limpiar los campos de entrada
            txtNombre.Clear();
            txtContraseña.Clear();

            // Recargar la lista de usuarios
            CargarUsuarios();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Inicio mainForm = new Inicio();
            mainForm.Show();

            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtNombre.Clear();
            txtContraseña.Clear();
        }
    }

}
