using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Interfaces
{
    public interface IGeneroService
    {
        Task<IToReturn<int>> InsertAsync(Generos genero);
        Task<IToReturn<int>> UpdateAsync(int id, Generos genero);
        Task<IToReturn<int>> CambiarEstadoAsync(int id);
        Task<IToReturnList<Generos>> BusquedaAsync(_Estado estado = _Estado.Todos);
        Task<IToReturn<Paginacion<List<Generos>>>> BusquedaPaginadaAsync(int page = 1, int pageSize = 10, _Estado estado = _Estado.Todos);
        Task<IToReturn<Generos>> GetByIdAsync(int id);
    }
}
