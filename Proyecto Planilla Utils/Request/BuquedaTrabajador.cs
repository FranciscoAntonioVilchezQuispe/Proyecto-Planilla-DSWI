using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_Utils.Request
{
    public class BuquedaTrabajador
    {
        public string Busqueda { get; set; }
        public _Estado Estado { get; set; } = _Estado.Todos;
    }
} 