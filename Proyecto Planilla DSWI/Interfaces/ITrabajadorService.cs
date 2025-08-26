using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Interfaces
{
    public interface ITrabajadorService
    {
        Task<IToReturn<int>> InsertAsync(Trabajadores trabajadores);
        Task<IToReturn<int>> UpdateAsync(int id, Trabajadores trabajadores);
        Task<IToReturn<int>> CambiarEstadoAsync(int id);
        Task<IToReturnList<Trabajadores>> BusquedaAsync(string busqueda, _Estado estado = _Estado.Todos);
        Task<IToReturn<Trabajadores>> GetByIdAsync(int id);        
    }
}
