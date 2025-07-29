using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_DSWI.Models
{
    public class Parametros: _Auditoria
    {
        [Key]
        public int IdParametro { get; set; }
        public decimal? RemBasico { get; set; }
        public decimal? PorcAsigancionFamiliar { get; set; }
        public decimal? PorcExtra1 { get; set; }
        public decimal? PorcExtra2 { get; set; }

    }
}
