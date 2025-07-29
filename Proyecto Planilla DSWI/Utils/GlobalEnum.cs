using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_DSWI.Utils
{
    public class GlobalEnum
    {
        public enum _TipoAccion
        {
            Ninguno = 0,
            Nuevo = 1,
            Modificar = 2,
            Eliminar = 3,
            CambiarEstado = 4,
        }
        public enum _Controller
        {
            Ninguno = 0,
            Cargos = 1,

        }
        public enum _Estado
        {
            Inactivo = 0,
            Activo = 1,
            Todos = 2,

        }
    }
} 