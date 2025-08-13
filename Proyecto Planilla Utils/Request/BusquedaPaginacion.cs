using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_Utils.Request
{
    public class BusquedaPaginacion
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 2;
        public _Estado estado { get; set; } = _Estado.Todos;
    }
}
