using Microsoft.AspNetCore.Mvc;
using subcats.customClass;
using subcats.dto;
using System.Data;

namespace subcats.Controllers
{
    public class CargosEmpleadosController : Controller
    {
        private readonly Dao _dao;

        public CargosEmpleadosController()
        {
            _dao = new Dao();
        }

        // GET: CargosEmpleados
        public IActionResult Index()
        {
            var cargos = _dao.ObtenerCargosEmpleados();
            return View(cargos);
        }

        // GET: CargosEmpleados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CargosEmpleados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CargoEmpleado cargo)
        {
            if (ModelState.IsValid)
            {
                if (_dao.CrearCargoEmpleado(cargo))
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al crear el cargo.");
            }
            return View(cargo);
        }

        // GET: CargosEmpleados/Edit/5
        public IActionResult Edit(int id)
        {
            var cargo = _dao.ObtenerCargoEmpleadoPorId(id);
            if (cargo == null)
            {
                return NotFound();
            }
            return View(cargo);
        }

        // POST: CargosEmpleados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CargoEmpleado cargo)
        {
            if (id != cargo.Id_cargo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (_dao.ActualizarCargoEmpleado(cargo))
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al actualizar el cargo.");
            }
            return View(cargo);
        }

        // GET: CargosEmpleados/Delete/5
        public IActionResult Delete(int id)
        {
            var cargo = _dao.ObtenerCargoEmpleadoPorId(id);
            if (cargo == null)
            {
                return NotFound();
            }
            return View(cargo);
        }

        // POST: CargosEmpleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_dao.EliminarCargoEmpleado(id))
            {
            }
            else
            {
                TempData["ErrorMessage"] = "Error al eliminar el cargo.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}