using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class EstadosCivilesController : Controller
    {
        private readonly IEstadoCivilService _estadoCivilService;
        private const int PageSize = 10;

        public EstadosCivilesController(IEstadoCivilService estadoCivilService)
        {
            _estadoCivilService = estadoCivilService;
        }

        public async Task<IActionResult> EstadosCiviles(int page = 1, _Estado estado = _Estado.Todos)
        {
            var result = await _estadoCivilService.BusquedaPaginadaAsync(page, PageSize, estado);

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
                return View(new List<EstadosCiviles>());
            }
        }

        public async Task<IActionResult> Manage(int? id)
        {
            if (id == null)
            {
                return View(new EstadosCiviles());
            }

            var result = await _estadoCivilService.GetByIdAsync(id.Value);
            if (result.Status == 200)
            {
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(EstadosCiviles));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(int? id, EstadosCiviles estadoCivil)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    var insertResult = await _estadoCivilService.InsertAsync(estadoCivil);

                    if (insertResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "Estado civil creado exitosamente.";
                        return RedirectToAction(nameof(EstadosCiviles));
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
                    var updateResult = await _estadoCivilService.UpdateAsync(id.Value, estadoCivil);

                    if (updateResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "Estado civil actualizado exitosamente.";
                        return RedirectToAction(nameof(EstadosCiviles));
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

            return View(estadoCivil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _estadoCivilService.CambiarEstadoAsync(id);

            if (result.Status == 200)
            {
                TempData["SuccessMessage"] = "Estado del estado civil cambiado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(EstadosCiviles));
        }
    }
}
