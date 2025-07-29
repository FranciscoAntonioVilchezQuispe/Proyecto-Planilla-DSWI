using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_DSWI.Models
{
    public class IngresosTrabajadores: _Auditoria
    {
        [Key]
        public int IdIngresoTrabajador { get; set; }
        public int? IdTrabajador { get; set; }
        public decimal? Remuneracion { get; set; }
        public decimal? Vale { get; set; }
        public decimal? BonifCargo { get; set; }
    }
}
