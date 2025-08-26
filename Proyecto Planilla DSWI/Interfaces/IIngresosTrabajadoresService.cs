using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Interfaces
{
    public interface IIngresosTrabajadoresService
    {
        Task<IToReturn<int>> InsertAsync(IngresosTrabajadores trabajadores);
        Task<IToReturn<int>> UpdateAsync(int id, IngresosTrabajadores trabajadores);
        Task<IToReturn<int>> CambiarEstadoAsync(int id);
        Task<IToReturnList<IngresosTrabajadores>> BusquedaAsync(_Estado estado = _Estado.Todos);
        Task<IToReturn<IngresosTrabajadores>> GetByIdAsync(int id);
    }
}
