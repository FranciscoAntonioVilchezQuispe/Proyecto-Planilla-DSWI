using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class GenerosController : Controller
    {
        private readonly IGeneroService _generoService;
        private const int PageSize = 10;

        public GenerosController(IGeneroService generoService)
        {
            _generoService = generoService;
        }

        public async Task<IActionResult> Generos(int page = 1, _Estado estado = _Estado.Todos)
        {
            var result = await _generoService.BusquedaPaginadaAsync(page, PageSize, estado);

            if (result.Status == 200)
            {
                var paginacion = result.Data;
                ViewBag.CurrentPage = paginacion.currentPage;
                ViewBag.TotalPages = paginacion.totalPages;
                ViewBag.PageSize = paginacion.pageSize;
                ViewBag.TotalItems = paginacion.totalItems;
                ViewBag.Estado = estado;
                return View(paginacion.data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return View(new List<Generos>());
            }
        }

        public async Task<IActionResult> Manage(int? id)
        {
            if (id == null)
            {
                return View(new Generos());
            }

            var result = await _generoService.GetByIdAsync(id.Value);
            if (result.Status == 200)
            {
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Generos));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(int? id, Generos genero)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    var insertResult = await _generoService.InsertAsync(genero);

                    if (insertResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "Género creado exitosamente.";
                        return RedirectToAction(nameof(Generos));
                    }
                    else
                    {
                        if (insertResult.Status == 412)
                        {
                            ModelState.AddModelError("", insertResult.Message);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = insertResult.Message;
                        }
                    }
                }
                else
                {
                    var updateResult = await _generoService.UpdateAsync(id.Value, genero);

                    if (updateResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "Género actualizado exitosamente.";
                        return RedirectToAction(nameof(Generos));
                    }
                    else
                    {
                        if (updateResult.Status == 412)
                        {
                            ModelState.AddModelError("", updateResult.Message);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = updateResult.Message;
                        }
                    }
                }
            }

            return View(genero);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _generoService.CambiarEstadoAsync(id);

            if (result.Status == 200)
            {
                TempData["SuccessMessage"] = "Estado del género cambiado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Generos));
        }
    }
}
