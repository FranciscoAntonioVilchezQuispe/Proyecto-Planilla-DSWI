using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_API.Data;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SituacionTrabajadorApiController : ControllerBase
    {
        private readonly SituacionLog _situacionLog;

        public SituacionTrabajadorApiController()
        {
            _situacionLog = new SituacionLog();
        }

        [HttpPost("Insert")]
        public IActionResult Insert(SituacionTrabajador obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var validationErrors = string.Join(", ",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    var validationError = new ToReturnValidation<int>(validationErrors);
                    return StatusCode(validationError.Status, validationError);
                }

                var res = _situacionLog.Insert(obj);
                var respuesta = new ToReturn<int>(res);
                return StatusCode(respuesta.Status, respuesta);
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<int>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpPut("Update/{id}")]
        public IActionResult Update(int id, SituacionTrabajador obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var validationErrors = string.Join(", ",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    var validationError = new ToReturnValidation<int>(validationErrors);
                    return StatusCode(validationError.Status, validationError);
                }

                obj.IdSituacion = id;
                var res = _situacionLog.Update(obj);

                if (res > 0)
                {
                    var respuesta = new ToReturn<int>(res);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<int>("No se pudo actualizar la situación de trabajador");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<int>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpDelete("CambiarEstado/{id}")]
        public IActionResult CambiarEstado(int id)
        {
            try
            {
                var res = _situacionLog.CambiarEstado(id);

                if (res > 0)
                {
                    var respuesta = new ToReturn<int>(res);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<int>("No se pudo cambiar el estado de la situación de trabajador");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<int>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpGet("Busqueda")]
        public IActionResult Busqueda([FromQuery] _Estado estado = _Estado.Todos)
        {
            try
            {
                var situaciones = _situacionLog.Busqueda(estado);

                if (situaciones != null && situaciones.Any())
                {
                    var respuesta = new ToReturnList<SituacionTrabajador>(situaciones);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontradoList<SituacionTrabajador>("No se encontraron situaciones de trabajador");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnErrorList<SituacionTrabajador>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpPost("BusquedaPaginada")]
        public IActionResult BusquedaPaginada(BusquedaPaginacion busqueda)
        {
            try
            {
                var todasLasSituaciones = _situacionLog.Busqueda(busqueda.estado).AsQueryable();
                var totalItems = todasLasSituaciones.Count();

                var situacionesPaginadas = todasLasSituaciones
                    .Skip((busqueda.page - 1) * busqueda.pageSize)
                    .Take(busqueda.pageSize)
                    .ToList();

                var resultado = new Paginacion<List<SituacionTrabajador>>
                {
                    data = situacionesPaginadas,
                    currentPage = busqueda.page,
                    pageSize = busqueda.pageSize,
                    totalItems = totalItems,
                    totalPages = (int)Math.Ceiling(totalItems / (double)busqueda.pageSize)
                };

                if (situacionesPaginadas.Any())
                {
                    var respuesta = new ToReturn<object>(resultado);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<object>("No se encontraron situaciones de trabajador para la página solicitada");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<object>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var situacion = _situacionLog.Busqueda(_Estado.Todos).FirstOrDefault(s => s.IdSituacion == id);

                if (situacion != null)
                {
                    var respuesta = new ToReturn<SituacionTrabajador>(situacion);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<SituacionTrabajador>("Situación de trabajador no encontrada");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<SituacionTrabajador>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }
    }
}
