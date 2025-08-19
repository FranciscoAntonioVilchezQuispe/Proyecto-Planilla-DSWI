using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_DSWI.Services.AsistenciaService;

namespace Proyecto_Planilla_DSWI.Interfaces
{
    public interface IAsistenciaService
    {
        Task<IToReturnList<AsistenciaTrabajadorResponse>> BuscarAsistenciaByPeriodoAsync(int año, int mes);
        Task<IToReturn<ArchivoResponse>> DescargarExcelAsync(int año, int mes, List<AsistenciaTrabajadorResponse> datos);
        Task<IToReturnList<AsistenciaTrabajadorResponse>> ProcesarExcelAsync(int año, int mes, IFormFile archivoExcel);
        Task<IToReturn<bool>> GrabarAsistenciaAsync(GrabarAsistencias datos);
    }
}
