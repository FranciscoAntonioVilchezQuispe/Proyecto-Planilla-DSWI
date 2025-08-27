using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using Proyecto_Planilla_Utils.Response;


namespace Proyecto_Planilla_DSWI.Interfaces
{
    public interface IPlanillaMensualService
    {
        Task<IToReturnList<PlanillaMensual>> CalcularPlanillaByPeriodoAsync(int año, int mes);
        Task<IToReturn<bool>> GrabarPlanillaAsync(List<PlanillaMensual> datos);
        Task<IToReturn<ArchivoResponse>> DescargarExcelAsync(int año, int mes);
        Task<IToReturnList<PlanillaMensual>> ListaAsync(int año, int mes);

        Task<string> GenerarBoletaAsync(int idTrabajador, int año, int mes);
        


    }
}
