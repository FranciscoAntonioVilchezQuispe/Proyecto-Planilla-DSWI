using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_Utils.Request
{
    public class ProcesarExcelRequest
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public IFormFile ArchivoExcel { get; set; }
    }
}
