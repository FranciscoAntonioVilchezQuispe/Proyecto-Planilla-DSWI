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
    public class SistemaPensionApiController : ControllerBase
    {
        private readonly SistemaPensionLog _sistemaPensionLog;

        public SistemaPensionApiController()
        {
            _sistemaPensionLog = new SistemaPensionLog();
        }

        [HttpPost("Insert")]
        public IActionResult Insert(SistemaPensiones obj)
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

                var res = _sistemaPensionLog.Insert(obj);
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
        public IActionResult Update(int id, SistemaPensiones obj)
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

                obj.IdSistemaPension = id;
                var res = _sistemaPensionLog.Update(obj);

                if (res > 0)
                {
                    var respuesta = new ToReturn<int>(res);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<int>("No se pudo actualizar el sistema de pensión");
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
                var res = _sistemaPensionLog.CambiarEstado(id);

                if (res > 0)
                {
                    var respuesta = new ToReturn<int>(res);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<int>("No se pudo cambiar el estado del sistema de pensión");
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
                var sistemas = _sistemaPensionLog.Busqueda(estado);

                if (sistemas != null && sistemas.Any())
                {
                    var respuesta = new ToReturnList<SistemaPensiones>(sistemas);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontradoList<SistemaPensiones>("No se encontraron sistemas de pensión");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnErrorList<SistemaPensiones>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpPost("BusquedaPaginada")]
        public IActionResult BusquedaPaginada(BusquedaPaginacion busqueda)
        {
            try
            {
                var todosLosSistemas = _sistemaPensionLog.Busqueda(busqueda.estado).AsQueryable();
                var totalItems = todosLosSistemas.Count();

                var sistemasPaginados = todosLosSistemas
                    .Skip((busqueda.page - 1) * busqueda.pageSize)
                    .Take(busqueda.pageSize)
                    .ToList();

                var resultado = new Paginacion<List<SistemaPensiones>>
                {
                    data = sistemasPaginados,
                    currentPage = busqueda.page,
                    pageSize = busqueda.pageSize,
                    totalItems = totalItems,
                    totalPages = (int)Math.Ceiling(totalItems / (double)busqueda.pageSize)
                };

                if (sistemasPaginados.Any())
                {
                    var respuesta = new ToReturn<object>(resultado);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<object>("No se encontraron sistemas de pensión para la página solicitada");
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
                var sistema = _sistemaPensionLog.Busqueda(_Estado.Todos).FirstOrDefault(s => s.IdSistemaPension == id);

                if (sistema != null)
                {
                    var respuesta = new ToReturn<SistemaPensiones>(sistema);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<SistemaPensiones>("Sistema de pensión no encontrado");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<SistemaPensiones>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }
    }
}
