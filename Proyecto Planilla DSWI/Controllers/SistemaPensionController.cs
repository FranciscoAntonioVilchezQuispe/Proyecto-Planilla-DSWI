using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_DSWI.Models;
using static Proyecto_Planilla_DSWI.Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class SistemaPensionController : Controller
    {
        private readonly SistemaPensionLog _sistemaPensionLog;
        private const int PageSize = 10;

        public SistemaPensionController()
        {
            _sistemaPensionLog = new SistemaPensionLog();
        }

        public IActionResult SistemaPension(int page = 1, _Estado estado = _Estado.Todos)
        {
            var sistemas = _sistemaPensionLog.Busqueda(estado).ToList();
            var totalItems = sistemas.Count;

            var paginatedItems = sistemas
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
                return View(new SistemaPensiones());
            }

            var sistema = _sistemaPensionLog.Busqueda(_Estado.Todos)
                .FirstOrDefault(s => s.IdSistemaPension == id);

            if (sistema == null)
            {
                TempData["ErrorMessage"] = "Sistema de pensión no encontrado";
                return RedirectToAction(nameof(SistemaPension));
            }

            return View(sistema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Manage(SistemaPensiones sistema)
        {
            if (!ModelState.IsValid)
            {
                return View(sistema);
            }

            try
            {
                int result;
                if (sistema.IdSistemaPension == 0)
                {
                    result = _sistemaPensionLog.Insert(sistema);
                    TempData["SuccessMessage"] = "Sistema de pensión creado exitosamente";
                }
                else
                {
                    result = _sistemaPensionLog.Update(sistema);
                    TempData["SuccessMessage"] = "Sistema de pensión actualizado exitosamente";
                }

                if (result > 0)
                {
                    return RedirectToAction(nameof(SistemaPension));
                }

                TempData["ErrorMessage"] = "No se pudo guardar el sistema de pensión";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return View(sistema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _sistemaPensionLog.CambiarEstado(id);
                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Estado del sistema de pensión cambiado exitosamente";
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

            return RedirectToAction(nameof(SistemaPension));
        }
    }
}
