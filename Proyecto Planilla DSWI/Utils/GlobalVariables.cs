using Proyecto_Planilla_DSWI.Utils.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_DSWI.Utils
{
    public class GlobalVariables
    {
        public static List<Genericos> Mes()
        {
            List<Genericos> arr = new List<Genericos>
            {
                new Genericos { Id = 1, Nombre = "Enero" },
                new Genericos { Id = 2, Nombre = "Febrero" },
                new Genericos { Id = 3, Nombre = "Marzo" },
                new Genericos { Id = 4, Nombre = "Abril" },
                new Genericos { Id = 5, Nombre = "Mayo" },
                new Genericos { Id = 6, Nombre = "Junio" },
                new Genericos { Id = 7, Nombre = "Julio" },
                new Genericos { Id = 8, Nombre = "Agosto" },
                new Genericos { Id = 9, Nombre = "Septiembre" },
                new Genericos { Id = 10, Nombre = "Octubre" },
                new Genericos { Id = 11, Nombre = "Noviembre" },
                new Genericos { Id = 12, Nombre = "Diciembre" }
            };
            return arr;
        }
        public static List<Genericos> AñoPeriodo()
        {
            List<Genericos> arr = new List<Genericos>();
            if (DateTime.Now > new DateTime(DateTime.Now.Year, 12, 10))
            {
                arr.Add(new Genericos { Id = DateTime.Now.Year + 1, Nombre = (DateTime.Now.Year + 1).ToString() });
            }
            for (int i = DateTime.Now.Year; i >= 2004; i--)
            {
                arr.Add(new Genericos { Id = i, Nombre = i.ToString() });
            }

            return arr;
        }
    }
} 