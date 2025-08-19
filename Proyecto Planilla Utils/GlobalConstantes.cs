namespace Proyecto_Planilla_Utils
{
    public class GlobalConstantes
    {
        //public static string Url = "https://localhost:7000/";
        public static string ApiCargo = $"/Cargos/";
        public static string ApiGenero = $"/Generos/";
        public static string ApiEstadoCivil = $"/EstadosCiviles/";
        public static string ApiParametro = $"/Parametros/";
        public static string ApiSistemaPensiones = $"/SistemaPension/";
        public static string ApiTipoDocumento = $"/TipoDocumento/";
        public static string ApiSituacionTrabajador = $"/Situacion/";
        public static string ApiTrabajador = $"/Trabajador/";
        public static string ApiIngresosTrabajadores = $"/IngresosTrabajadores/";
        public static string ApiAsistenciaTrabajador = $"/AsistenciaTrabajador/";
        public static string ApiPlanillaMensual = $"/PlanillaMensual/";

        #region Auditoria
        public const string AuditoriaUpdate = ",FecUltimaModificacion = @FecUltimaModificacion ";
        public const string AuditoriaInsertColumna = ",Activo,FecCreacion";
        public const string AuditoriaInsertValues = ",@Activo,@FecCreacion";
        public const string SelectIdentity = "select SCOPE_IDENTITY();";
        #endregion
        #region alias
        public const string AliasCabecera = "IdPadre";
        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public sealed class AliasAttribute : Attribute
        {
            public string Name { get; }
            public AliasAttribute(string name) => Name = name;
        }
        #endregion
    }
}
