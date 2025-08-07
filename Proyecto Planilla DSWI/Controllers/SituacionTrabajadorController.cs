using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_DSWI.Models;
using static Proyecto_Planilla_DSWI.Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class SituacionTrabajadorController : Controller
    {
        private readonly SituacionLog _situacionLog;
        private const int PageSize = 10;

        public SituacionTrabajadorController()
        {
            _situacionLog = new SituacionLog();
        }

        public IActionResult SituacionTrabajador(int page = 1, _Estado estado = _Estado.Todos)
        {
            var situaciones = _situacionLog.Busqueda(estado).ToList();
            var totalItems = situaciones.Count;

            var paginatedItems = situaciones
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            ViewBag.PageSize = PageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.Estado = estado;

            return View(paginatedItems);
        }

        public IActionResult Manage(int id = 0)
        {
            if (id == 0)
            {
                return View(new SituacionTrabajador());
            }

            var situacion = _situacionLog.Busqueda(_Estado.Todos)
                .FirstOrDefault(s => s.IdSituacion == id);

            if (situacion == null)
            {
                TempData["ErrorMessage"] = "Situación de trabajador no encontrada";
                return RedirectToAction(nameof(SituacionTrabajador));
            }

            return View(situacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Manage(SituacionTrabajador situacion)
        {
            if (!ModelState.IsValid)
            {
                return View(situacion);
            }

            try
            {
                int result;
                if (situacion.IdSituacion == 0)
                {
                    result = _situacionLog.Insert(situacion);
                    TempData["SuccessMessage"] = "Situación de trabajador creada exitosamente";
                }
                else
                {
                    result = _situacionLog.Update(situacion);
                    TempData["SuccessMessage"] = "Situación de trabajador actualizada exitosamente";
                }

                if (result > 0)
                {
                    return RedirectToAction(nameof(SituacionTrabajador));
                }

                TempData["ErrorMessage"] = "No se pudo guardar la situación de trabajador";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return View(situacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _situacionLog.CambiarEstado(id);
                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Estado de la situación de trabajador cambiado exitosamente";
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

            return RedirectToAction(nameof(SituacionTrabajador));
        }
    }
}
