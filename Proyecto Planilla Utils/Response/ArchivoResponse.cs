using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_Utils.Response
{
    public class ArchivoResponse
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; }
    }
    public class DescargarExcelResponse
    {
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public string ContentType { get; set; }
    }

}
