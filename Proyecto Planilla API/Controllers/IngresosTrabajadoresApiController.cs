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
    public class IngresosTrabajadoresApiController : ControllerBase
    {
        private readonly IngresosTrabajadoresLog _ingresosTrabajadoresLog;

        public IngresosTrabajadoresApiController()
        {
            _ingresosTrabajadoresLog = new IngresosTrabajadoresLog();
        }

        [HttpPost("Insert")]
        public IActionResult Insert(IngresosTrabajadores obj)
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

                var res = _ingresosTrabajadoresLog.Insert(obj);
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
        public IActionResult Update(int id, IngresosTrabajadores obj)
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

                obj.IdTrabajador = id;
                var res = _ingresosTrabajadoresLog.Update(obj);

                if (res > 0)
                {
                    var respuesta = new ToReturn<int>(res);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<int>($"No se pudo actualizar el registro del ingreso trabajador");
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
                var res = _ingresosTrabajadoresLog.CambiarEstado(id);

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
        public IActionResult Busqueda(_Estado estado = _Estado.Todos)
        {
            try
            {
                var listaIngresoTrabajador = _ingresosTrabajadoresLog.Busqueda(estado).ToList();
                var respuesta = new ToReturnList<IngresosTrabajadores>(listaIngresoTrabajador);
                return StatusCode(respuesta.Status, respuesta);
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<int>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpGet("GetById/{id}")]
        public IActionResult BusquedaOne(int Id)
        {
            try
            {
                var ingresoTrabajador = _ingresosTrabajadoresLog.BusquedaOne(Id);

                if (ingresoTrabajador != null)
                {
                    var respuesta = new ToReturn<IngresosTrabajadores>(ingresoTrabajador);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontrado<IngresosTrabajadores>("Ingresos trabajador no encontrada");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<IngresosTrabajadores>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }
    }
}
