using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_API.Data;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;

namespace Proyecto_Planilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrosApiController : ControllerBase
    {
        private readonly ParametrosLog _parametrosLog;

        public ParametrosApiController()
        {
            _parametrosLog = new ParametrosLog();
        }

        [HttpPost("Insert")]
        public IActionResult Insert(Parametros obj)
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

                var res = _parametrosLog.Insert(obj);
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
        public IActionResult Update(int id, Parametros obj)
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

                obj.IdParametro = id;
                var res = _parametrosLog.Update(obj);

                if (res > 0)
                {
                    var respuesta = new ToReturn<int>(res);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<int>("No se pudo actualizar los parámetros");
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
                var res = _parametrosLog.CambiarEstado(id);

                if (res > 0)
                {
                    var respuesta = new ToReturn<int>(res);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<int>("No se pudo cambiar el estado de los parámetros");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<int>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpGet("BusquedaOne")]
        public IActionResult BusquedaOne()
        {
            try
            {
                var parametro = _parametrosLog.BusquedaOne();

                if (parametro != null)
                {
                    var respuesta = new ToReturn<Parametros>(parametro);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<Parametros>("No se encontraron parámetros");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<Parametros>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }
    }
}
