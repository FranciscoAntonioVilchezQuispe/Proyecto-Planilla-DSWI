using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_DSWI.Models
{
    public class SistemaPensiones: _Auditoria
    {
        [Key]
        public int IdSistemaPension { get; set; }
        public string Nombre { get; set; }
        public decimal? Aporte { get; set; }
        public decimal? Comision { get; set; }
        public decimal? Prima { get; set; }

    }
}
