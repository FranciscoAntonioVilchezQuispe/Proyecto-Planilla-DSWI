using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_Entidades
{
    public class SituacionTrabajador: _Auditoria
    {
        [Key]
        public int IdSituacion { get; set; }
        public string Nombre { get; set; }

    }
}
