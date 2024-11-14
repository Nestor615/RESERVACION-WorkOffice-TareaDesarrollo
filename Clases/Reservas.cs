using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESERVACION_WorkOffice_TareaDesarrollo.Clases
{
    internal class Reservas
    {
        public int IdReserva { get; set; }

        public DateTime FechaReserva { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public string PersonaReserva { get; set; }

        public string NombresAsistente { get; set; }

        public decimal TotalPago { get; set; }

        public Reservas(int idReserva, DateTime fechaReserva, TimeSpan horaInicio, TimeSpan horaFin, string personaReserva, string nombresAsistente, decimal totalPago)
        {
            IdReserva = idReserva;
            FechaReserva = fechaReserva;
            HoraInicio = horaInicio;
            HoraFin = horaFin;
            PersonaReserva = personaReserva;
            NombresAsistente = nombresAsistente;
            TotalPago = totalPago;
        }
    }
}
