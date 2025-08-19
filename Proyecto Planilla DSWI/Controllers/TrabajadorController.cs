using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using static Org.BouncyCastle.Math.EC.ECCurve;



namespace Proyecto_Planilla_DSWI.Controllers
{
    public class TrabajadorController : Controller
    {
        private readonly IConfiguration _config;

        private readonly TrabajadorLog _trabajadorLog;
        private readonly CargoLog _cargoLog;
        private readonly SituacionLog _situacionLog;
        private readonly TipoDocumentoLog _tipoDocumentoLog;
        private readonly GenerosLog _generosLog;
        private readonly EstadosCivilesLog _estadosCivilesLog;
        private readonly SistemaPensionLog _sistemaPensionLog;
        private readonly IngresosTrabajadoresLog _ingresoLog;

        List<Cargos> ArrCargos = new List<Cargos>();
        List<SituacionTrabajador> ArrSituacion = new List<SituacionTrabajador>();
        List<TipoDocumentos> ArrTpoDocumento = new List<TipoDocumentos>();
        List<Generos> ArrGenero = new List<Generos>();
        List<EstadosCiviles> ArrEstadocivil = new List<EstadosCiviles>();
        List<SistemaPensiones> ArrSistemaPensiones = new List<SistemaPensiones>();
        
        public TrabajadorController()
        {
            _trabajadorLog = new TrabajadorLog();
            _cargoLog = new CargoLog();
            _situacionLog = new SituacionLog();
            _tipoDocumentoLog = new TipoDocumentoLog();
            _generosLog = new GenerosLog();
            _estadosCivilesLog = new EstadosCivilesLog();
            _sistemaPensionLog = new SistemaPensionLog();
            _ingresoLog = new IngresosTrabajadoresLog();
        }

        public async Task CargarParametros()
        {
            ArrCargos = (List<Cargos>)_cargoLog.Busqueda(GlobalEnum._Estado.Activo);
            ArrSituacion = (List<SituacionTrabajador>)_situacionLog.Busqueda(GlobalEnum._Estado.Activo);
            ArrTpoDocumento = (List<TipoDocumentos>)_tipoDocumentoLog.Busqueda(GlobalEnum._Estado.Activo);
            ArrGenero = (List<Generos>)_generosLog.Busqueda(GlobalEnum._Estado.Activo);
            ArrEstadocivil = (List<EstadosCiviles>)_estadosCivilesLog.Busqueda(GlobalEnum._Estado.Activo);
            ArrSistemaPensiones = (List<SistemaPensiones>)_sistemaPensionLog.Busqueda(GlobalEnum._Estado.Activo);
        }

        public async Task<IActionResult> NuevoRegistro()
        {
            try
            {
                await CargarParametros();
                ViewBag.h1 = "Registro de Trabajador";
                ViewBag.tipoDocumento = new SelectList(ArrTpoDocumento, "IdTipoDocumento", "Nombre");
                ViewBag.genero = new SelectList(ArrGenero, "IdGenero", "Nombre");
                ViewBag.estCivil = new SelectList(ArrEstadocivil, "IdEstadoCivil", "Nombre");
                ViewBag.situacion = new SelectList(ArrSituacion, "IdSituacion", "Nombre");
                ViewBag.cargo = new SelectList(ArrCargos, "IdCargo", "Nombre");
                ViewBag.sistPension = new SelectList(ArrSistemaPensiones, "IdSistemaPension", "Nombre");

                return View("RegistroTrabajador", new Trabajadores());
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                throw;
            }

        }

        public async Task<IActionResult> EditarRegistro(string busqueda)
        {
            try
            {
                await CargarParametros();
                ViewBag.h1 = "Editar Trabajador";
                ViewBag.tipoDocumento = new SelectList(ArrTpoDocumento, "IdTipoDocumento", "Nombre");
                ViewBag.genero = new SelectList(ArrGenero, "IdGenero", "Nombre");
                ViewBag.estCivil = new SelectList(ArrEstadocivil, "IdEstadoCivil", "Nombre");
                ViewBag.situacion = new SelectList(ArrSituacion, "IdSituacion", "Nombre");
                ViewBag.cargo = new SelectList(ArrCargos, "IdCargo", "Nombre");
                ViewBag.sistPension = new SelectList(ArrSistemaPensiones, "IdSistemaPension", "Nombre");

                var Obj = await GetTrabajadores(busqueda);

                return View("RegistroTrabajador", Obj.FirstOrDefault());
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Index(string busqueda, int page = 1)
        {
            List<Trabajadores> Lista = new List<Trabajadores>();

            try
            {
                Lista = await GetTrabajadores(busqueda);

                int totalRegistros = Lista.Count;
                int regisroPorPagina = 5;

                int totalPaginas = (int)Math.Ceiling((double)totalRegistros / regisroPorPagina);
                int omitir = (page - 1) * regisroPorPagina;
                ViewBag.totalPaginas = totalPaginas;

                return View(Lista.Skip(omitir).Take(regisroPorPagina));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<List<Trabajadores>> GetTrabajadores(string busqueda)
        {
            List<Trabajadores> Lista = new List<Trabajadores>();
            var objBusqueda = new BuquedaTrabajador { Busqueda = busqueda, Estado = GlobalEnum._Estado.Todos };
            try
            {
                Lista = _trabajadorLog.Busqueda(objBusqueda).ToList();

                return Lista;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistroTrabajador(Trabajadores objTrabajador)
        {
            int intResult = 0;            
            try
            {
                if (objTrabajador.IdTrabajador == 0)
                    intResult = _trabajadorLog.Insert(objTrabajador);

                else
                    intResult = _trabajadorLog.Update(objTrabajador);


                if (intResult == 0)
                    throw new Exception("No se realizó el registro.");
                else
                    return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult IngresoMensual(int id)
        {
            var obj = _ingresoLog.BusquedaOne(id);

            if(obj == null)
                return PartialView();
            else
                return PartialView(obj);
        }

        public IActionResult RegistroMensual(IngresosTrabajadores objIngreso)
        {
            int intResult = 0;
            if (objIngreso.IdIngresoTrabajador == 0)
                intResult = _ingresoLog.Insert(objIngreso);

            else
                intResult = _ingresoLog.Update(objIngreso);

            if (intResult == 0)
                throw new Exception("No se realizó el registro.");
            else
                objIngreso.IdIngresoTrabajador = intResult;

            return PartialView(objIngreso);
        }
    }
}
