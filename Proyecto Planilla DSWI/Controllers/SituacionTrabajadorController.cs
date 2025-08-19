using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class SituacionTrabajadorController : Controller
    {
        private readonly ISituacionTrabajadorService _situacionTrabajadorService;
        private const int PageSize = 10;

        public SituacionTrabajadorController(ISituacionTrabajadorService situacionTrabajadorService)
        {
            _situacionTrabajadorService = situacionTrabajadorService;
        }

        // GET: SituacionTrabajador
        public async Task<IActionResult> SituacionTrabajador(int page = 1, _Estado estado = _Estado.Todos)
        {
            var result = await _situacionTrabajadorService.BusquedaPaginadaAsync(page, PageSize, estado);

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
                return View(new List<SituacionTrabajador>());
            }
        }

        // GET: SituacionTrabajador/Manage
        public async Task<IActionResult> Manage(int? id)
        {
            if (id == null)
            {
                return View(new SituacionTrabajador());
            }

            var result = await _situacionTrabajadorService.GetByIdAsync(id.Value);
            if (result.Status == 200)
            {
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(SituacionTrabajador));
            }
        }

        // POST: SituacionTrabajador/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(int? id, SituacionTrabajador situacion)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    // Crear nueva situación de trabajador
                    var insertResult = await _situacionTrabajadorService.InsertAsync(situacion);

                    if (insertResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "Situación de trabajador creada exitosamente.";
                        return RedirectToAction(nameof(SituacionTrabajador));
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
                    // Actualizar situación de trabajador existente
                    var updateResult = await _situacionTrabajadorService.UpdateAsync(id.Value, situacion);

                    if (updateResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "Situación de trabajador actualizada exitosamente.";
                        return RedirectToAction(nameof(SituacionTrabajador));
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

            return View(situacion);
        }

        // POST: SituacionTrabajador/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _situacionTrabajadorService.CambiarEstadoAsync(id);

            if (result.Status == 200)
            {
                TempData["SuccessMessage"] = "Estado de la situación de trabajador cambiado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(SituacionTrabajador));
        }
    }
}
