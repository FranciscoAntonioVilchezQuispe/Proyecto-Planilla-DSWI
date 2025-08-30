using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class TipoDocumentoController : Controller
    {
        private readonly ITipoDocumentoService _tipoDocumentoService;
        private const int PageSize = 10;

        public TipoDocumentoController(ITipoDocumentoService tipoDocumentoService)
        {
            _tipoDocumentoService = tipoDocumentoService;
        }

        // GET: TipoDocumento
        public async Task<IActionResult> TipoDocumento(int page = 1, _Estado estado = _Estado.Todos)
        {
            var result = await _tipoDocumentoService.BusquedaPaginadaAsync(page, PageSize, estado);

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
                return View(new List<TipoDocumentos>());
            }
        }

        // GET: TipoDocumento/Manage
        public async Task<IActionResult> Manage(int? id)
        {
            if (id == null)
            {
                return View(new TipoDocumentos());
            }

            var result = await _tipoDocumentoService.GetByIdAsync(id.Value);
            if (result.Status == 200)
            {
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(TipoDocumento));
            }
        }

        // POST: TipoDocumento/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(int? id, TipoDocumentos tipoDocumento)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    // Crear nuevo tipo de documento
                    var insertResult = await _tipoDocumentoService.InsertAsync(tipoDocumento);

                    if (insertResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "Tipo de documento creado exitosamente.";
                        return RedirectToAction(nameof(TipoDocumento));
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
                    // Actualizar tipo de documento existente
                    var updateResult = await _tipoDocumentoService.UpdateAsync(id.Value, tipoDocumento);

                    if (updateResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "Tipo de documento actualizado exitosamente.";
                        return RedirectToAction(nameof(TipoDocumento));
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

            return View(tipoDocumento);
        }

        // POST: TipoDocumento/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tipoDocumentoService.CambiarEstadoAsync(id);

            if (result.Status == 200)
            {
                TempData["SuccessMessage"] = "Estado del tipo de documento cambiado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(TipoDocumento));
        }
    }
}
