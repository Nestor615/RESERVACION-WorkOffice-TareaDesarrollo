using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESERVACION_WorkOffice_TareaDesarrollo.Clases
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
        public string Contraseña { get; set; }

        public Usuario(int idUsuario, string nombre, string rol, string contraseña)
        {
            IdUsuario = idUsuario;
            Nombre = nombre;
            Rol = rol;
            Contraseña = contraseña;
        }
    }

}
