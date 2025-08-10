namespace Proyecto_Planilla_DSWI.Utils
{
    public class GlobalConstantes
    {
        public static string Url = "https://localhost:7000/";
        public static string ApiCargo = $"{Url}api/Cargos/";
        public static string ApiGenero = $"{Url}api/Generos/";
        public static string ApiEstadoCivil = $"{Url}api/EstadosCiviles/";
        public static string ApiParametro = $"{Url}api/Parametros/";
        public static string ApiSistemaPensiones = $"{Url}api/SistemaPension/";
        public static string ApiTipoDocumento = $"{Url}api/TipoDocumento/";
        public static string ApiSituacionTrabajador = $"{Url}api/Situacion/";
        public static string ApiTrabajador = $"{Url}api/Trabajador/";
        public static string ApiIngresosTrabajadores = $"{Url}api/IngresosTrabajadores/";
        public static string ApiAsistenciaTrabajador = $"{Url}api/AsistenciaTrabajador/";
        public static string ApiPlanillaMensual = $"{Url}api/PlanillaMensual/";

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
