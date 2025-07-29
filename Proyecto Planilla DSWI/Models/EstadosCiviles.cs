using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_DSWI.Models
{
    public class EstadosCiviles:_Auditoria
    {
        [Key]
        public int IdEstadoCivil { get; set; }
        public string Nombre { get; set; }

    }
}
