using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_DSWI.Models;
using static Proyecto_Planilla_DSWI.Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class EstadosCivilesController : Controller
    {
        private readonly EstadosCivilesLog _estadosCivilesLog;
        private const int PageSize = 10;

        public EstadosCivilesController()
        {
            _estadosCivilesLog = new EstadosCivilesLog();
        }

        public IActionResult EstadosCiviles(int page = 1, _Estado estado = _Estado.Todos)
        {
            var estadosCiviles = _estadosCivilesLog.Busqueda(estado).ToList();
            var totalItems = estadosCiviles.Count;

            var paginatedItems = estadosCiviles
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
                return View(new EstadosCiviles());
            }

            var estadoCivil = _estadosCivilesLog.Busqueda(_Estado.Todos)
                .FirstOrDefault(e => e.IdEstadoCivil == id);

            if (estadoCivil == null)
            {
                TempData["ErrorMessage"] = "Estado civil no encontrado";
                return RedirectToAction(nameof(EstadosCiviles));
            }

            return View(estadoCivil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Manage(EstadosCiviles estadoCivil)
        {
            if (!ModelState.IsValid)
            {
                return View(estadoCivil);
            }

            try
            {
                int result;
                if (estadoCivil.IdEstadoCivil == 0)
                {
                    result = _estadosCivilesLog.Insert(estadoCivil);
                    TempData["SuccessMessage"] = "Estado civil creado exitosamente";
                }
                else
                {
                    result = _estadosCivilesLog.Update(estadoCivil);
                    TempData["SuccessMessage"] = "Estado civil actualizado exitosamente";
                }

                if (result > 0)
                {
                    return RedirectToAction(nameof(EstadosCiviles));
                }

                TempData["ErrorMessage"] = "No se pudo guardar el estado civil";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return View(estadoCivil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _estadosCivilesLog.CambiarEstado(id);
                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Estado del estado civil cambiado exitosamente";
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

            return RedirectToAction(nameof(EstadosCiviles));
        }
    }
}
