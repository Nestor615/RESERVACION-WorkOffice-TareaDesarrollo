using MySql.Data.MySqlClient;
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
    public partial class Inicio : Form
    {
        public string connectionString = "Server=localhost;Database=reservacion;User ID=root;";

        ReservasService reservasService = new ReservasService();

        public Inicio()
        {          
            InitializeComponent();
            CargarReservas();
            CargarDatosReservasMenus();
            CargarSalas();
            CalcularTotalPago();     
        }

        private void CargarReservas()
        {
            List<Reservas> reservas = reservasService.ObtenerReservas();
            dgvReservas.AutoGenerateColumns = true;
            dgvReservas.DataSource = reservas;
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            CalcularTotalPago();

            int idSala = ((KeyValuePair<int, string>)cmbSalas.SelectedItem).Key;
            DateTime fechaReserva = dtpFechaReserva.Value;
            TimeSpan horaInicio = dtpHoraInicio.Value.TimeOfDay;
            TimeSpan horaFin = dtpHoraFin.Value.TimeOfDay;
            string personaReserva = txtPersona.Text;
            string nombresAsistentes = txtAsistentes.Text;
            decimal totalPago = Convert.ToDecimal(lblTotalPago.Text.Replace("Total a Pagar: $", ""));           

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insertar reserva
                        string insertReserva = "INSERT INTO Reservas (idSala, fechaReserva, horaInicio, horaFin, personaReserva, nombresAsistente, totalPago) " +
                                               "VALUES (@idSala, @fechaReserva, @horaInicio, @horaFin, @personaReserva, @nombresAsistentes, @totalPago)";
                        MySqlCommand cmdReserva = new MySqlCommand(insertReserva, connection, transaction);
                        cmdReserva.Parameters.AddWithValue("@idSala", idSala);
                        cmdReserva.Parameters.AddWithValue("@fechaReserva", fechaReserva);
                        cmdReserva.Parameters.AddWithValue("@horaInicio", horaInicio);
                        cmdReserva.Parameters.AddWithValue("@horaFin", horaFin);
                        cmdReserva.Parameters.AddWithValue("@personaReserva", personaReserva);
                        cmdReserva.Parameters.AddWithValue("@nombresAsistentes", nombresAsistentes);
                        cmdReserva.Parameters.AddWithValue("@totalPago", totalPago);
                        cmdReserva.ExecuteNonQuery();

                        int idReserva = (int)cmdReserva.LastInsertedId;

                        // Insertar relación en ReservasMenus
                        foreach (DataGridViewRow row in dgvMenu.Rows)
                        {
                            // Verifica que la fila no es nueva y que las celdas necesarias no estén vacías
                            if (!row.IsNewRow && row.Cells["idMenu"].Value != null && row.Cells["cantidadPersonas"].Value != null)
                            {
                                int idMenu = Convert.ToInt32(row.Cells["idMenu"].Value);
                                int cantidadPersonas = Convert.ToInt32(row.Cells["cantidadPersonas"].Value);

                                // Verifica si el idMenu existe en la tabla Menus
                                string checkMenuQuery = "SELECT COUNT(*) FROM Menus WHERE idMenu = @idMenu";
                                MySqlCommand checkMenuCmd = new MySqlCommand(checkMenuQuery, connection);
                                checkMenuCmd.Parameters.AddWithValue("@idMenu", idMenu);
                                int menuCount = Convert.ToInt32(checkMenuCmd.ExecuteScalar());

                                if (menuCount > 0) // Si el menú existe
                                {
                                    string insertReservaMenu = "INSERT INTO ReservasMenus (idReserva, idMenu, cantidadPersonas) VALUES (@idReserva, @idMenu, @cantidadPersonas)";
                                    MySqlCommand cmdReservaMenu = new MySqlCommand(insertReservaMenu, connection, transaction);
                                    cmdReservaMenu.Parameters.AddWithValue("@idReserva", idReserva);
                                    cmdReservaMenu.Parameters.AddWithValue("@idMenu", idMenu);
                                    cmdReservaMenu.Parameters.AddWithValue("@cantidadPersonas", cantidadPersonas);
                                    cmdReservaMenu.ExecuteNonQuery();
                                }
                                else
                                {
                                    MessageBox.Show($"El Menú con ID {idMenu} no existe en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    transaction.Rollback();
                                    return;
                                }
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("Reserva realizada con éxito.", "Reserva", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        CargarReservas();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error al realizar la reserva: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void CalcularTotalPago()
        {
            decimal totalPago = 0;

            foreach (DataGridViewRow row in dgvMenu.Rows)
            {
                // Asegurarse de que la fila no es nueva y que ambas celdas tienen valores válidos
                if (!row.IsNewRow && row.Cells["idMenu"].Value != null && row.Cells["cantidadPersonas"].Value != null)
                {
                    int idMenu = Convert.ToInt32(row.Cells["idMenu"].Value);
                    int cantidadPersonas = Convert.ToInt32(row.Cells["cantidadPersonas"].Value);

                    // Obtener el precio del menú basado en idMenu
                    decimal precioMenu = 0;
                    switch (idMenu)
                    {
                        case 1:
                            precioMenu = 10.00m;
                            break;
                        case 2:
                            precioMenu = 12.00m;
                            break;
                        case 3:
                            precioMenu = 15.00m;
                            break;
                        default:
                            MessageBox.Show("Menú desconocido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }

                    totalPago += precioMenu * cantidadPersonas;
                }
            }

            // Actualizar la etiqueta con el total
            lblTotalPago.Text = $"Total a Pagar: ${totalPago:F2}";
        }



        private void CargarSalas()
        {
            string query = "SELECT idSala, nombreSala FROM Salas";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                // Diccionario para almacenar las salas
                Dictionary<int, string> salasDict = new Dictionary<int, string>();

                while (reader.Read())
                {
                    int idSala = reader.GetInt32("idSala");
                    string nombreSala = reader.GetString("nombreSala");
                    salasDict.Add(idSala, nombreSala);
                }

                cmbSalas.DataSource = new BindingSource(salasDict, null);
                cmbSalas.DisplayMember = "Value";
                cmbSalas.ValueMember = "Key";
            }
        }

        // Llama a este método para cargar los datos cuando abras el formulario o cargues una reserva
        private void CargarDatosReservasMenus()
        {
            dgvMenu.Rows.Clear(); // Limpia las filas antes de cargar nuevos datos
            string query = "SELECT idMenu, cantidadPersonas FROM ReservasMenus WHERE idReserva = @idReserva";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@idReserva", idMenu);

                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int idMenu = reader.GetInt32("idMenu");
                    int cantidadPersonas = reader.GetInt32("cantidadPersonas");
                    dgvMenu.Rows.Add(idMenu, cantidadPersonas);
                }
                dgvMenu.AutoGenerateColumns = true;
            }
        }



        private void cmbSalas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSalas.SelectedItem != null)
            {
                int idSalaSeleccionada = ((KeyValuePair<int, string>)cmbSalas.SelectedItem).Key;
            }
        }
        private void dgvMenu_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CalcularTotalPago();
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUsuarios usuarioForm = new FormUsuarios();
            usuarioForm.Show();

            Close();
        }
    }
}
