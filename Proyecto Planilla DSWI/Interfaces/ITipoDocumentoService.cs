using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Interfaces
{
    public interface ITipoDocumentoService
    {
        Task<IToReturn<int>> InsertAsync(TipoDocumentos tipoDocumento);
        Task<IToReturn<int>> UpdateAsync(int id, TipoDocumentos tipoDocumento);
        Task<IToReturn<int>> CambiarEstadoAsync(int id);
        Task<IToReturnList<TipoDocumentos>> BusquedaAsync(_Estado estado = _Estado.Todos);
        Task<IToReturn<Paginacion<List<TipoDocumentos>>>> BusquedaPaginadaAsync(int page = 1, int pageSize = 10, _Estado estado = _Estado.Todos);
        Task<IToReturn<TipoDocumentos>> GetByIdAsync(int id);

    }
}
