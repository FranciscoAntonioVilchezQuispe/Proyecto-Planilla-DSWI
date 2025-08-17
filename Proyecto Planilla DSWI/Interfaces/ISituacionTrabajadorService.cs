using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Interfaces
{
    public interface ISituacionTrabajadorService
    {
        Task<IToReturn<int>> InsertAsync(SituacionTrabajador situacionTrabajador);
        Task<IToReturn<int>> UpdateAsync(int id, SituacionTrabajador situacionTrabajador);
        Task<IToReturn<int>> CambiarEstadoAsync(int id);
        Task<IToReturnList<SituacionTrabajador>> BusquedaAsync(_Estado estado = _Estado.Todos);
        Task<IToReturn<Paginacion<List<SituacionTrabajador>>>> BusquedaPaginadaAsync(int page = 1, int pageSize = 10, _Estado estado = _Estado.Todos);
        Task<IToReturn<SituacionTrabajador>> GetByIdAsync(int id);
    }
}
