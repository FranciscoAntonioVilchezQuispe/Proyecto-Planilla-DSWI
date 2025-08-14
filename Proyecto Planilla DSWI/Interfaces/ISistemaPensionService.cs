using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Interfaces
{
    public interface ISistemaPensionService
    {
        Task<IToReturn<int>> InsertAsync(SistemaPensiones sistemaPension);
        Task<IToReturn<int>> UpdateAsync(int id, SistemaPensiones sistemaPension);
        Task<IToReturn<int>> CambiarEstadoAsync(int id);
        Task<IToReturnList<SistemaPensiones>> BusquedaAsync(_Estado estado = _Estado.Todos);
        Task<IToReturn<Paginacion<List<SistemaPensiones>>>> BusquedaPaginadaAsync(int page = 1, int pageSize = 10, _Estado estado = _Estado.Todos);
        Task<IToReturn<SistemaPensiones>> GetByIdAsync(int id);
    }
}
