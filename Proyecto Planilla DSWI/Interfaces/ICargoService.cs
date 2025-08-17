using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Interfaces
{
    public interface ICargoService
    {
        Task<IToReturn<int>> InsertAsync(Cargos cargo);
        Task<IToReturn<int>> UpdateAsync(int id, Cargos cargo);
        Task<IToReturn<int>> CambiarEstadoAsync(int id);
        Task<IToReturnList<Cargos>> BusquedaAsync(_Estado estado = _Estado.Todos);
        Task<IToReturn<Paginacion<List<Cargos>>>> BusquedaPaginadaAsync(int page = 1, int pageSize = 2, _Estado estado = _Estado.Todos);
        Task<IToReturn<Cargos>> GetByIdAsync(int id);
    }
}
