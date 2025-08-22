using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using System.Drawing.Printing;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class TrabajadorController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ITrabajadorService _trabajadorService;
        private readonly ICargoService _cargoService;
        private readonly ISituacionTrabajadorService _situacionTrabajadorService;
        private readonly ITipoDocumentoService _tipoDocumentoService;
        private readonly IGeneroService _generoService;
        private readonly IEstadoCivilService _estadoCivilService;
        private readonly ISistemaPensionService _sistemaPensionService;

        List<Cargos> ArrCargos = new List<Cargos>();
        List<SituacionTrabajador> ArrSituacion = new List<SituacionTrabajador>();
        List<TipoDocumentos> ArrTpoDocumento = new List<TipoDocumentos>();
        List<Generos> ArrGenero = new List<Generos>();
        List<EstadosCiviles> ArrEstadocivil = new List<EstadosCiviles>();
        List<SistemaPensiones> ArrSistemaPensiones = new List<SistemaPensiones>();
        
        public TrabajadorController(IConfiguration config, ITrabajadorService trabajadorService, ICargoService cargoService,
            ISituacionTrabajadorService situacionTrabajadorService,
            ITipoDocumentoService tipoDocumentoService,
            IGeneroService generoService,
            IEstadoCivilService estadoCivilService,
            ISistemaPensionService sistemaPensionService)
        {
            _config = config;
            _trabajadorService = trabajadorService;
            _cargoService = cargoService;
            _situacionTrabajadorService = situacionTrabajadorService;
            _tipoDocumentoService = tipoDocumentoService;
            _generoService = generoService;
            _estadoCivilService = estadoCivilService;
            _sistemaPensionService = sistemaPensionService;
        }

        public async Task CargarParametros()
        {
            ArrCargos = await GetCargo();
            ArrSituacion = await GetSituacion();
            ArrTpoDocumento = await GetTipoDocumentos();
            ArrGenero = await GetGenero();
            ArrEstadocivil = await GetEstadoCivil();
            ArrSistemaPensiones = await GetSistPensiones();
        }

        public async Task<List<Cargos>> GetCargo()
        {
            var result = await _cargoService.BusquedaAsync(_Estado.Activo);
            return result.Status == 200 ? result.Data.ToList() : new List<Cargos>();
        }

        private async Task<List<SituacionTrabajador>> GetSituacion()
        {
            var result = await _situacionTrabajadorService.BusquedaAsync(_Estado.Activo);
            return result.Status == 200 ? result.Data.ToList() : new List<SituacionTrabajador>();
        }

        private async Task<List<TipoDocumentos>> GetTipoDocumentos()
        {
            var result = await _tipoDocumentoService.BusquedaAsync(_Estado.Activo);
            return result.Status == 200 ? result.Data.ToList() : new List<TipoDocumentos>();
        }

        private async Task<List<Generos>> GetGenero()
        {
            var result = await _generoService.BusquedaAsync(_Estado.Activo);
            return result.Status == 200 ? result.Data.ToList() : new List<Generos>();
        }

        private async Task<List<EstadosCiviles>> GetEstadoCivil()
        {
            var result = await _estadoCivilService.BusquedaAsync(_Estado.Activo);
            return result.Status == 200 ? result.Data.ToList() : new List<EstadosCiviles>();
        }

        private async Task<List<SistemaPensiones>> GetSistPensiones()
        {
            var result = await _sistemaPensionService.BusquedaAsync(_Estado.Activo);
            return result.Status == 200 ? result.Data.ToList() : new List<SistemaPensiones>();
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

        public async Task<IActionResult> EditarRegistro(int id)
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

                var result = await _trabajadorService.GetByIdAsync(id);

                return View("RegistroTrabajador", result.Data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Index(string busqueda = "", int page = 1)
        {
            List<Trabajadores> Lista = new List<Trabajadores>();

            try
            {
                var listado = new List<Trabajadores>();
                var objBusqueda = new BuquedaTrabajador { Busqueda = busqueda, Estado = GlobalEnum._Estado.Todos };

                var result = await _trabajadorService.BusquedaAsync(busqueda, GlobalEnum._Estado.Todos);
                Lista = result.Data.ToList();

                int totalRegistros = Lista.Count;
                int regisroPorPagina = 10;

                int totalPaginas = (int)Math.Ceiling((double)totalRegistros / regisroPorPagina);
                int omitir = (page - 1) * regisroPorPagina;
                ViewBag.totalPaginas = totalPaginas;

                return View("index", result.Data.Skip(omitir).Take(regisroPorPagina));

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistroTrabajador(Trabajadores objTrabajador)
        {
            //if (ModelState.IsValid)
            //{
                if (objTrabajador.IdTrabajador == 0)
                {
                    var insertResult = await _trabajadorService.InsertAsync(objTrabajador);

                    if (insertResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "El trabajador creada exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        if (insertResult.Status == 412)
                        {
                            ModelState.AddModelError("", insertResult.Message);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = insertResult.Message;
                        }
                    }
                }
                else
                {                    
                    var updateResult = await _trabajadorService.UpdateAsync(objTrabajador.IdTrabajador, objTrabajador);

                    if (updateResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "El trabajador actualizada exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        if (updateResult.Status == 412)
                        {
                            ModelState.AddModelError("", updateResult.Message);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = updateResult.Message;
                        }
                    }
                }
            //}

            return View("RegistroTrabajador", objTrabajador);
        }

        public async Task<IActionResult> EliminarRegistro(int id)
        {
            var obj = _trabajadorService.CambiarEstadoAsync(id);
            return RedirectToAction("index");
        }

        //public IActionResult IngresoMensual(int id)
        //{
        //    var obj = _ingresoLog.BusquedaOne(id);

        //    if(obj == null)
        //        return PartialView();
        //    else
        //        return PartialView(obj);
        //}

        //public IActionResult RegistroMensual(IngresosTrabajadores objIngreso)
        //{
        //    int intResult = 0;
        //    if (objIngreso.IdIngresoTrabajador == 0)
        //        intResult = _ingresoLog.Insert(objIngreso);

        //    else
        //        intResult = _ingresoLog.Update(objIngreso);

        //    if (intResult == 0)
        //        throw new Exception("No se realizó el registro.");
        //    else
        //        objIngreso.IdIngresoTrabajador = intResult;

        //    return PartialView(objIngreso);
        //}

    }
}
