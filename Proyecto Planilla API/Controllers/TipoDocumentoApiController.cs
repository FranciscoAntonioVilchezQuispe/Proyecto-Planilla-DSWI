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
    public class TipoDocumentoApiController : ControllerBase
    {
        private readonly TipoDocumentoLog _tipoDocumentoLog;

        public TipoDocumentoApiController()
        {
            _tipoDocumentoLog = new TipoDocumentoLog();
        }

        [HttpPost("Insert")]
        public IActionResult Insert(TipoDocumentos obj)
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

                var res = _tipoDocumentoLog.Insert(obj);
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
        public IActionResult Update(int id, TipoDocumentos obj)
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

                obj.IdTipoDocumento = id;
                var res = _tipoDocumentoLog.Update(obj);

                if (res > 0)
                {
                    var respuesta = new ToReturn<int>(res);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<int>("No se pudo actualizar el tipo de documento");
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
                var res = _tipoDocumentoLog.CambiarEstado(id);

                if (res > 0)
                {
                    var respuesta = new ToReturn<int>(res);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<int>("No se pudo cambiar el estado del tipo de documento");
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
                var tiposDocumento = _tipoDocumentoLog.Busqueda(estado);

                if (tiposDocumento != null && tiposDocumento.Any())
                {
                    var respuesta = new ToReturnList<TipoDocumentos>(tiposDocumento);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontradoList<TipoDocumentos>("No se encontraron tipos de documento");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnErrorList<TipoDocumentos>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpPost("BusquedaPaginada")]
        public IActionResult BusquedaPaginada(BusquedaPaginacion busqueda)
        {
            try
            {
                var todosLosTipos = _tipoDocumentoLog.Busqueda(busqueda.estado).AsQueryable();
                var totalItems = todosLosTipos.Count();

                var tiposPaginados = todosLosTipos
                    .Skip((busqueda.page - 1) * busqueda.pageSize)
                    .Take(busqueda.pageSize)
                    .ToList();

                var resultado = new Paginacion<List<TipoDocumentos>>
                {
                    data = tiposPaginados,
                    currentPage = busqueda.page,
                    pageSize = busqueda.pageSize,
                    totalItems = totalItems,
                    totalPages = (int)Math.Ceiling(totalItems / (double)busqueda.pageSize)
                };

                if (tiposPaginados.Any())
                {
                    var respuesta = new ToReturn<object>(resultado);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<object>("No se encontraron tipos de documento para la página solicitada");
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
                var tipoDocumento = _tipoDocumentoLog.Busqueda(_Estado.Todos).FirstOrDefault(t => t.IdTipoDocumento == id);

                if (tipoDocumento != null)
                {
                    var respuesta = new ToReturn<TipoDocumentos>(tipoDocumento);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<TipoDocumentos>("Tipo de documento no encontrado");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<TipoDocumentos>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }
    }
}
