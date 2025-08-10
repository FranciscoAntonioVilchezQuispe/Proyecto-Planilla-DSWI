using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Proyecto_Planilla_DSWI.Models;
using Proyecto_Planilla_DSWI.Utils;
using Proyecto_Planilla_DSWI.Utils.Request;
using Newtonsoft.Json;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class TrabajadorController : Controller
    {
        public HttpClient _httpClient;
        Trabajadores objTrabajador = new Trabajadores();
        List<Cargos> ArrCargos = new List<Cargos>();
        List<SituacionTrabajador> ArrSituacion = new List<SituacionTrabajador>();
        List<TipoDocumentos> ArrTpoDocumento = new List<TipoDocumentos>();
        List<Generos> ArrGenero = new List<Generos>();
        List<EstadosCiviles> ArrEstadocivil = new List<EstadosCiviles>();
        List<SistemaPensiones> ArrSistemaPensiones = new List<SistemaPensiones>();

        public TrabajadorController()
        {
            _httpClient = new HttpClient();
        }
        async Task CargarParametros()
        {
            try
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response = await _httpClient.GetAsync($"{GlobalConstantes.ApiParametro}ParametrosFormularioTrabajador");
                if (!response.IsSuccessStatusCode) throw new Exception("Error: " + response.RequestMessage.ToString());
                using (HttpContent content = response.Content)
                {
                    var obj = JsonConvert.DeserializeObject<dynamic>(await content.ReadAsStringAsync());

                    switch (JsonConvert.DeserializeObject<int>(System.Convert.ToString(obj["status"])))
                    {
                        case 200:
                            var objarr = JsonConvert.DeserializeObject<dynamic>(System.Convert.ToString(obj["data"]));
                            ArrCargos = JsonConvert.DeserializeObject<List<Cargos>>(System.Convert.ToString(objarr["cargos"]));
                            ArrSituacion = JsonConvert.DeserializeObject<List<SituacionTrabajador>>(System.Convert.ToString(objarr["situaciones"]));
                            ArrTpoDocumento = JsonConvert.DeserializeObject<List<TipoDocumentos>>(System.Convert.ToString(objarr["tipoDocumentos"]));
                            ArrGenero = JsonConvert.DeserializeObject<List<Generos>>(System.Convert.ToString(objarr["generos"]));
                            ArrEstadocivil = JsonConvert.DeserializeObject<List<EstadosCiviles>>(System.Convert.ToString(objarr["estadosCiviles"]));
                            ArrSistemaPensiones = JsonConvert.DeserializeObject<List<SistemaPensiones>>(System.Convert.ToString(objarr["sistemaPensiones"]));
                            break;

                        case 500: throw new Exception(System.Convert.ToString(obj["message"]));

                    }
                }
            }
            catch (Exception ex)
            {
                throw; //mensaje_error(ex);
            }
        }

        public async Task<List<Trabajadores>> GetTrabajadores(string busqueda)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            List<Trabajadores> Lista = new List<Trabajadores>();

            try
            {
                response = await _httpClient.PostAsJsonAsync($"{GlobalConstantes.ApiTrabajador}BusquedaTrabajadores", new BuquedaTrabajador { Busqueda = busqueda, Estado = GlobalEnum._Estado.Todos });
                if (!response.IsSuccessStatusCode) throw new Exception("Error: " + response.RequestMessage.ToString());
                using (HttpContent content = response.Content)
                {
                    var obj = JsonConvert.DeserializeObject<dynamic>(await content.ReadAsStringAsync());

                    switch (JsonConvert.DeserializeObject<int>(System.Convert.ToString(obj["status"])))
                    {
                        case 200:
                            Lista = JsonConvert.DeserializeObject<List<Trabajadores>>(System.Convert.ToString(obj["data"]));
                            break;

                        case 500: throw new Exception(System.Convert.ToString(obj["message"]));
                    }
                }

                return Lista;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> NuevoRegistro()
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
                throw;
            }
        }


        public async Task<IActionResult> Index(string busqueda, int page = 1)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            BuquedaTrabajador objBusqueda = new BuquedaTrabajador();
            List<Trabajadores> Lista = new List<Trabajadores>();
            _httpClient = new HttpClient();
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
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistroTrabajador(Trabajadores newTrabajador)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                if (newTrabajador.IdTrabajador == 0)
                {
                    response = await _httpClient.PostAsJsonAsync($"{GlobalConstantes.ApiTrabajador}Insert", newTrabajador);
                }
                else
                {
                    response = await _httpClient.PostAsJsonAsync($"{GlobalConstantes.ApiTrabajador}Update", newTrabajador);
                }

                if (!response.IsSuccessStatusCode) throw new Exception("Error: " + response.RequestMessage.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }


            return RedirectToAction("Index");
        }
    }
}
