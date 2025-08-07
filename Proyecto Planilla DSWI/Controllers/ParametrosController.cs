using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_DSWI.Models;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class ParametrosController : Controller
    {
        private readonly ParametrosLog _parametrosLog;

        public ParametrosController()
        {
            _parametrosLog = new ParametrosLog();
        }

        public IActionResult Manage()
        {
            var parametros = _parametrosLog.BusquedaOne();

            if (parametros == null)
            {
                // Si no existe registro, creamos uno nuevo
                parametros = new Parametros();
            }

            return View(parametros);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Manage(Parametros parametros)
        {
            if (!ModelState.IsValid)
            {
                return View(parametros);
            }

            try
            {
                int result;
                if (parametros.IdParametro == 0)
                {
                    // Insertar nuevo registro
                    result = _parametrosLog.Insert(parametros);
                    TempData["SuccessMessage"] = "Parámetros creados exitosamente";
                }
                else
                {
                    // Actualizar registro existente
                    result = _parametrosLog.Update(parametros);
                    TempData["SuccessMessage"] = "Parámetros actualizados exitosamente";
                }

                if (result > 0)
                {
                    return RedirectToAction(nameof(Manage));
                }

                TempData["ErrorMessage"] = "No se pudieron guardar los parámetros";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return View(parametros);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado()
        {
            try
            {
                // Obtenemos el único registro
                var parametro = _parametrosLog.BusquedaOne();
                if (parametro == null)
                {
                    TempData["ErrorMessage"] = "No existe registro de parámetros para cambiar estado";
                    return RedirectToAction(nameof(Manage));
                }

                var result = _parametrosLog.CambiarEstado(parametro.IdParametro);
                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Estado de los parámetros cambiado exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se pudo cambiar el estado";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(Manage));
        }
    }
}
