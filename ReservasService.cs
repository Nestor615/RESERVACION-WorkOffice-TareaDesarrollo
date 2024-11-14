using MySql.Data.MySqlClient;
using RESERVACION_WorkOffice_TareaDesarrollo.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESERVACION_WorkOffice_TareaDesarrollo
{
    internal class ReservasService
    {
        private string connectionString = "Server=localhost;Database=reservacion;User ID=root;";

        public List<Reservas> ObtenerReservas()
        {
            List<Reservas> listaReservas = new List<Reservas>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT idReserva, fechaReserva, horaInicio, horaFin, personaReserva, nombresAsistente, totalPago FROM reservas";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Reservas reservas = new Reservas(
                                reader.GetInt32("idReserva"),
                                reader.GetDateTime("fechaReserva"),
                                reader.GetTimeSpan("horaInicio"),
                                reader.GetTimeSpan("horaFin"),
                                reader.GetString("personaReserva"),
                                reader.GetString("nombresAsistente"),
                                reader.GetDecimal("totalPago")
                            );
                            listaReservas.Add(reservas);
                        }
                    }
                }
            }
            return listaReservas;
        }
    }
}
