using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_Entidades;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class TipoDocumentoController : Controller
    {
        private readonly TipoDocumentoLog _tipoDocumentoLog;
        private const int PageSize = 10;

        public TipoDocumentoController()
        {
            _tipoDocumentoLog = new TipoDocumentoLog();
        }

        public IActionResult TipoDocumento(int page = 1, _Estado estado = _Estado.Todos)
        {
            var tiposDocumento = _tipoDocumentoLog.Busqueda(estado).ToList();
            var totalItems = tiposDocumento.Count;

            var paginatedItems = tiposDocumento
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
                return View(new TipoDocumentos());
            }

            var tipoDocumento = _tipoDocumentoLog.Busqueda(_Estado.Todos)
                .FirstOrDefault(t => t.IdTipoDocumento == id);

            if (tipoDocumento == null)
            {
                TempData["ErrorMessage"] = "Tipo de documento no encontrado";
                return RedirectToAction(nameof(TipoDocumento));
            }

            return View(tipoDocumento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Manage(TipoDocumentos tipoDocumento)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoDocumento);
            }

            try
            {
                int result;
                if (tipoDocumento.IdTipoDocumento == 0)
                {
                    result = _tipoDocumentoLog.Insert(tipoDocumento);
                    TempData["SuccessMessage"] = "Tipo de documento creado exitosamente";
                }
                else
                {
                    result = _tipoDocumentoLog.Update(tipoDocumento);
                    TempData["SuccessMessage"] = "Tipo de documento actualizado exitosamente";
                }

                if (result > 0)
                {
                    return RedirectToAction(nameof(TipoDocumento));
                }

                TempData["ErrorMessage"] = "No se pudo guardar el tipo de documento";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return View(tipoDocumento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _tipoDocumentoLog.CambiarEstado(id);
                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Estado del tipo de documento cambiado exitosamente";
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

            return RedirectToAction(nameof(TipoDocumento));
        }
    }
}
