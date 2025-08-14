using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;

namespace Proyecto_Planilla_DSWI.Interfaces
{
    public interface IParametroService
    {
        Task<IToReturn<int>> InsertAsync(Parametros parametro);
        Task<IToReturn<int>> UpdateAsync(int id, Parametros parametro);
        Task<IToReturn<int>> CambiarEstadoAsync(int id);
        Task<IToReturn<Parametros>> BusquedaOneAsync();
    }
}
