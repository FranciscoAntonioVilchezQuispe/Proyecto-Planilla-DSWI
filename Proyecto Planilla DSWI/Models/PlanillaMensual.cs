using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_DSWI.Models
{
    public class PlanillaMensual: _Auditoria
    {
        [Key]
        public decimal IdPlanillaMensual { get; set; }
        public int? Año { get; set; }
        public int? Mes { get; set; }
        public int? IdTrabajador { get; set; }
        public int? IdSituacion { get; set; }
        public int? IdCargo { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public int? IdSistemaPension { get; set; }
        public int? IdEstadoCivil { get; set; }
        public short? Hijos { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public decimal? SueldoBasico { get; set; }
        public decimal? PorcHoraExtra1 { get; set; }
        public decimal? PorcHoraExtra2 { get; set; }
        public decimal? PorcDescansoTrab { get; set; }
        public decimal? PorcFeriadoTrab { get; set; }
        public decimal? PorcAsigFamiliar { get; set; }
        public decimal? nHorasNormal { get; set; }
        public decimal? nHorasExtra1 { get; set; }
        public decimal? nHorasExtra2 { get; set; }
        public short? nDiasTrab { get; set; }
        public short? nDiasDescansos { get; set; }
        public short? nFeriadosTrab { get; set; }
        public short? nDescansosTrab { get; set; }
        public short? nDiasInasistencias { get; set; }
        public decimal? HaberBasico { get; set; }
        public decimal? ValesEmpleado { get; set; }
        public decimal? vHorasExtra1 { get; set; }
        public decimal? vHorasExtra2 { get; set; }
        public decimal? vAsigFamiliar { get; set; }
        public decimal? vDescansoTrab { get; set; }
        public decimal? vFeriadoTrab { get; set; }
        public decimal? BonificacionCargo { get; set; }
        public decimal? BonificacionMovilidad { get; set; }
        public decimal? CanastaNavidad { get; set; }
        public decimal? Escolaridad { get; set; }
        public decimal? DiaTrabajador { get; set; }
        public decimal? TotalIngreso { get; set; }
        public decimal? Renta5ta { get; set; }
        public decimal? DescuentoJud1 { get; set; }
        public decimal? DescuentoJud2 { get; set; }
        public decimal? DescuentoJud3 { get; set; }
        public decimal? OtrosAdelantos { get; set; }
        public decimal? AdelantoCaja { get; set; }
        public decimal? AdelantoQuincena { get; set; }
        public decimal? AdelantoVac { get; set; }
        public decimal? AdelantoGratificacion { get; set; }
        public decimal? AdelantoLiquidacion { get; set; }
        public decimal? AdelantoCTS { get; set; }
        public decimal? PorcAporte { get; set; }
        public decimal? Aporte { get; set; }
        public decimal? PorcComision { get; set; }
        public decimal? Comision { get; set; }
        public decimal? PorcPrima { get; set; }
        public decimal? Prima { get; set; }
        public decimal? OTDSeg { get; set; }
        public decimal? OTDPacifico { get; set; }
        public decimal? IdBanco1 { get; set; }
        public decimal? Prestamo1 { get; set; }
        public decimal? Tardanza { get; set; }
        public decimal? TotalDescuento { get; set; }
        public decimal? PorcEsSalud { get; set; }
        public decimal? EsSalud { get; set; }
        public decimal? AccidenteTrab { get; set; }
        public decimal? Senati { get; set; }
        public decimal? SeguroVidaLey { get; set; }
        public decimal? TotalNeto { get; set; }
        public string TotalNetoCad { get; set; }
        public decimal? TotalNetoBoleta { get; set; }
        public string TotalNetoBoletaCad { get; set; }
    }
}
