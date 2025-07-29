using System.ComponentModel.DataAnnotations;

namespace Proyecto_Planilla_DSWI.Models
{
    public class Trabajadores:_Auditoria
    {
        [Key]
        public int IdTrabajador { get; set; }
        public int IdTipoDocumento { get; set; }
        public string Documento { get; set; } // VARCHAR(11)
        public string Nombres { get; set; } // VARCHAR(50)
        public string ApellidoPaterno { get; set; } // VARCHAR(50)
        public string ApellidoMaterno { get; set; } // VARCHAR(50)
        public int IdGenero { get; set; }
        public int IdEstadoCivil { get; set; }
        public string Direccion { get; set; } // VARCHAR(120)
        public string Email { get; set; } // VARCHAR(120), nullable
        public int Hijos { get; set; }
        public int IdCargo { get; set; }
        public DateTime FecNacimiento { get; set; } // DATE
        public DateTime FecIngreso { get; set; } // DATE
        public int IdSituacion { get; set; }
        public int IdSistemaPension { get; set; }
        public byte[] Foto { get; set; } // IMAGE, nullable

    }
}
