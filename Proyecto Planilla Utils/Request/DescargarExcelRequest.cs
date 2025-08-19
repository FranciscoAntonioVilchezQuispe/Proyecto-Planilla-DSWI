using Proyecto_Planilla_Utils.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_Utils.Request
{
    public class DescargarExcelRequest
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public List<AsistenciaTrabajadorResponse> Datos { get; set; }
    }
}
