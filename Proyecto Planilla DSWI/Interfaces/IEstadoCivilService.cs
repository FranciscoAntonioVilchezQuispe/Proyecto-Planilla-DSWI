using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Interfaces
{
    public interface IEstadoCivilService
    {
        Task<IToReturn<int>> InsertAsync(EstadosCiviles estadoCivil);
        Task<IToReturn<int>> UpdateAsync(int id, EstadosCiviles estadoCivil);
        Task<IToReturn<int>> CambiarEstadoAsync(int id);
        Task<IToReturnList<EstadosCiviles>> BusquedaAsync(_Estado estado = _Estado.Todos);
        Task<IToReturn<Paginacion<List<EstadosCiviles>>>> BusquedaPaginadaAsync(int page = 1, int pageSize = 10, _Estado estado = _Estado.Todos);
        Task<IToReturn<EstadosCiviles>> GetByIdAsync(int id);
    }
}
