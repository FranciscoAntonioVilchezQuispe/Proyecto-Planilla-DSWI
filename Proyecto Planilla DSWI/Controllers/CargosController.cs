using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_DSWI.Models;
using System.Drawing.Printing;
using static Proyecto_Planilla_DSWI.Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class CargosController : Controller
    {
        private readonly CargoLog _cargoLog;
        private const int PageSize = 2; // Tamaño de página por defecto
        public CargosController()
        {
            _cargoLog = new CargoLog();
        }

        // GET: Cargos
        public IActionResult Cargos(int page = 1, _Estado estado = _Estado.Todos)
        {
            var cargosQuery = _cargoLog.Busqueda(estado).AsQueryable();

            // Aplicar paginación
            var cargos = cargosQuery
            .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Calcular total de páginas
            var totalItems = cargosQuery.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Pasar datos a la vista
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = PageSize;
            ViewBag.Estado = estado;
            ViewBag.TotalItems = totalItems;
            return View(cargos);
        }

        // GET: Cargos/Manage
        public IActionResult Manage(int? id)
        {
            if (id == null)
            {
                // Vista de creación
                return View(new Cargos());
            }

            var cargo = _cargoLog.Busqueda(_Estado.Todos).FirstOrDefault(c => c.IdCargo == id);
            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo);
        }

        // POST: Cargos/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Manage(int? id, Cargos cargo)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    // Crear nuevo cargo
                    _cargoLog.Insert(cargo);
                    TempData["SuccessMessage"] = "Cargo creado exitosamente.";
                }
                else
                {
                    // Actualizar cargo existente
                    cargo.IdCargo = id.Value;
                    _cargoLog.Update(cargo);
                    TempData["SuccessMessage"] = "Cargo actualizado exitosamente.";
                }
                return RedirectToAction(nameof(Cargos));
            }
            return View(cargo);
        }

        // POST: Cargos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var result = _cargoLog.CambiarEstado(id);
            if (result > 0)
            {
                TempData["SuccessMessage"] = "Estado del cargo cambiado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo cambiar el estado del cargo.";
            }
            return RedirectToAction(nameof(Cargos));
        }
    }
}
