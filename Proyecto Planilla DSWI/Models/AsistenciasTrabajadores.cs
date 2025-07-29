using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_DSWI.Models
{
    public class AsistenciasTrabajadores:_Auditoria
    {
        [Key]
        public int IdAsistencia { get; set; }
        public int? IdTrabajador { get; set; }
        public int? Año { get; set; }
        public int? Mes { get; set; }
        public int? DiasLaborales { get; set; }
        public int? DiasDescanso { get; set; }
        public int? DiasInasistencia { get; set; }
        public int? DiasFeriados { get; set; }
        public decimal? HorasExtra25 { get; set; }
        public decimal? HorasExtra35 { get; set; }
    }
}
