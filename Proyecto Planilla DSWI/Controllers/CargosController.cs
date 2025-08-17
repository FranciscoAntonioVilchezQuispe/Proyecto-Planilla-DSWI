using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using System.Drawing.Printing;
using System.Text.Json;
using static Proyecto_Planilla_Utils.GlobalEnum;


namespace Proyecto_Planilla_DSWI.Controllers
{
    public class CargosController : Controller
    {
        private readonly ICargoService _cargoApiService;
        private const int PageSize = 2;

        public CargosController(ICargoService cargoApiService)
        {
            _cargoApiService = cargoApiService;
        }

        // GET: Cargos
        public async Task<IActionResult> Cargos(int page = 1, _Estado estado = _Estado.Todos)
        {
            var result = await _cargoApiService.BusquedaPaginadaAsync(page, PageSize, estado);

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
                return View(new List<Cargos>());
            }
        }

        // GET: Cargos/Manage
        public async Task<IActionResult> Manage(int? id)
        {
            if (id == null)
            {
                // Vista de creación
                return View(new Cargos());
            }

            var result = await _cargoApiService.GetByIdAsync(id.Value);
            if (result.Status == 200)
            {
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Cargos));
            }
        }

        // POST: Cargos/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(int? id, Cargos cargo)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    // Crear nuevo cargo
                    var insertResult = await _cargoApiService.InsertAsync(cargo);

                    if (insertResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "Cargo creado exitosamente.";
                        return RedirectToAction(nameof(Cargos));
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
                    // Actualizar cargo existente
                    var updateResult = await _cargoApiService.UpdateAsync(id.Value, cargo);

                    if (updateResult.Status == 200)
                    {
                        TempData["SuccessMessage"] = "Cargo actualizado exitosamente.";
                        return RedirectToAction(nameof(Cargos));
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

            return View(cargo);
        }

        // POST: Cargos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _cargoApiService.CambiarEstadoAsync(id);

            if (result.Status == 200)
            {
                TempData["SuccessMessage"] = "Estado del cargo cambiado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Cargos));
        }


    }
}
