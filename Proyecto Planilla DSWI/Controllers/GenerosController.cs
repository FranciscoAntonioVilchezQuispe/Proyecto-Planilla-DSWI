using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_DSWI.Models;
using static Proyecto_Planilla_DSWI.Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class GenerosController : Controller
    {
        private readonly GenerosLog _generosLog;
        private const int PageSize = 10;

        public GenerosController()
        {
            _generosLog = new GenerosLog();
        }

        public IActionResult Generos(int page = 1, _Estado estado = _Estado.Todos)
        {
            var generos = _generosLog.Busqueda(estado).ToList();
            var totalItems = generos.Count;

            var paginatedItems = generos
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
                return View(new Generos());
            }

            var genero = _generosLog.Busqueda(_Estado.Todos)
                .FirstOrDefault(g => g.IdGenero == id);

            if (genero == null)
            {
                TempData["ErrorMessage"] = "Género no encontrado";
                return RedirectToAction(nameof(Generos));
            }

            return View(genero);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Manage(Generos genero)
        {
            if (!ModelState.IsValid)
            {
                return View(genero);
            }

            try
            {
                int result;
                if (genero.IdGenero == 0)
                {
                    result = _generosLog.Insert(genero);
                    TempData["SuccessMessage"] = "Género creado exitosamente";
                }
                else
                {
                    result = _generosLog.Update(genero);
                    TempData["SuccessMessage"] = "Género actualizado exitosamente";
                }

                if (result > 0)
                {
                    return RedirectToAction(nameof(Generos));
                }

                TempData["ErrorMessage"] = "No se pudo guardar el género";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return View(genero);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _generosLog.CambiarEstado(id);
                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Estado del género cambiado exitosamente";
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

            return RedirectToAction(nameof(Generos));
        }
    }
}
