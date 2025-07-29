using Proyecto_Planilla_DSWI.Models;

namespace Proyecto_Planilla_DSWI.Data
{
    public class AuditoriaLog
    {
        public void SetAuditFieldsForInsert(_Auditoria entity)
        {
            entity.FecCreacion = DateTime.Now;
            entity.Activo = true;
        }

        public void SetAuditFieldsForUpdate(_Auditoria entity)
        {
            entity.FecCreacion = DateTime.Now;
            entity.FecUltimaModificacion = DateTime.Now;
        }
    }
} 