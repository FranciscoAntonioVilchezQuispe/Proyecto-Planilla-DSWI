using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class SistemaPensionController : Controller
    {
        private readonly ISistemaPensionService _sistemaPensionService;
        private const int PageSize = 10;

        public SistemaPensionController(ISistemaPensionService sistemaPensionService)
        {
            _sistemaPensionService = sistemaPensionService;
        }

        // GET: SistemaPension
        public async Task<IActionResult> SistemaPension(int page = 1, _Estado estado = _Estado.Todos)
        {
            var result = await _sistemaPensionService.BusquedaPaginadaAsync(page, PageSize, estado);

            if (result.Status == 200)
            {
                // Deserializar la respuesta paginada
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
                return View(new List<SistemaPensiones>());
            }
        }

        // GET: SistemaPension/Manage
        public async Task<IActionResult> Manage(int? id)
        {
            if (id == null)
            {
                // Vista de creación
                return View(new SistemaPensiones());
            }

            var result = await _sistemaPensionService.GetByIdAsync(id.Value);
            if (result.Status == 200)
            {
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(SistemaPension));
            }
        }

        // POST: SistemaPension/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(int? id, SistemaPensiones sistema)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    // Crear nuevo sistema de pensión
                    var insertResult = await _sistemaPensionService.InsertAsync(sistema);

                    if (insertResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "Sistema de pensión creado exitosamente.";
                        return RedirectToAction(nameof(SistemaPension));
                    }
                    else
                    {
                        if (insertResult.Status == 412) // Validation error
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
                    // Actualizar sistema de pensión existente
                    var updateResult = await _sistemaPensionService.UpdateAsync(id.Value, sistema);

                    if (updateResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "Sistema de pensión actualizado exitosamente.";
                        return RedirectToAction(nameof(SistemaPension));
                    }
                    else
                    {
                        if (updateResult.Status == 412) // Validation error
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

            return View(sistema);
        }

        // POST: SistemaPension/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _sistemaPensionService.CambiarEstadoAsync(id);

            if (result.Status == 200)
            {
                TempData["SuccessMessage"] = "Estado del sistema de pensión cambiado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(SistemaPension));
        }
    }
}
