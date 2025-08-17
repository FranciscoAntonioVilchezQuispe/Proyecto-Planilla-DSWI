using Proyecto_Planilla_Entidades;

namespace Proyecto_Planilla_API.Data
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