using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_Utils.Response
{
    public class Paginacion<T>
    {

            public T data { get; set; }
            public int currentPage { get; set; }
            public int pageSize { get; set; }
            public int totalItems { get; set; }
            public int totalPages { get; set; }

    }
}
