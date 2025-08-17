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
    public class GenerosApiController : ControllerBase
    {
        private readonly GenerosLog _generosLog;

        public GenerosApiController()
        {
            _generosLog = new GenerosLog();
        }

        [HttpPost("Insert")]
        public IActionResult Insert(Generos obj)
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

                var res = _generosLog.Insert(obj);
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
        public IActionResult Update(int id, Generos obj)
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

                obj.IdGenero = id;
                var res = _generosLog.Update(obj);

                if (res > 0)
                {
                    var respuesta = new ToReturn<int>(res);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<int>("No se pudo actualizar el género");
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
                var res = _generosLog.CambiarEstado(id);

                if (res > 0)
                {
                    var respuesta = new ToReturn<int>(res);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<int>("No se pudo cambiar el estado del género");
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
                var generos = _generosLog.Busqueda(estado);

                if (generos != null && generos.Any())
                {
                    var respuesta = new ToReturnList<Generos>(generos);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontradoList<Generos>("No se encontraron géneros");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnErrorList<Generos>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpPost("BusquedaPaginada")]
        public IActionResult BusquedaPaginada(BusquedaPaginacion busqueda)
        {
            try
            {
                var todosLosGeneros = _generosLog.Busqueda(busqueda.estado).AsQueryable();
                var totalItems = todosLosGeneros.Count();

                var generosPaginados = todosLosGeneros
                    .Skip((busqueda.page - 1) * busqueda.pageSize)
                    .Take(busqueda.pageSize)
                    .ToList();

                var resultado = new Paginacion<List<Generos>>
                {
                    data = generosPaginados,
                    currentPage = busqueda.page,
                    pageSize = busqueda.pageSize,
                    totalItems = totalItems,
                    totalPages = (int)Math.Ceiling(totalItems / (double)busqueda.pageSize)
                };

                if (generosPaginados.Any())
                {
                    var respuesta = new ToReturn<object>(resultado);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<object>("No se encontraron géneros para la página solicitada");
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
                var genero = _generosLog.Busqueda(_Estado.Todos).FirstOrDefault(g => g.IdGenero == id);

                if (genero != null)
                {
                    var respuesta = new ToReturn<Generos>(genero);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<Generos>("Género no encontrado");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<Generos>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }
    }
}
