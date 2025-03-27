using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using subcats.dto;
using subcats.customClass;
using System;
using System.Collections.Generic;

namespace subcats.Controllers
{
    public class CarritoController : Controller
    {
        private readonly Dao _db;

        public CarritoController()
        {
            _db = new Dao();
        }

        // GET: Carrito
        public IActionResult Index()
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        // GET: Carrito/Checkout
        public IActionResult Checkout()
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        // POST: Carrito/Procesar
        [HttpPost]
        public IActionResult Procesar([FromBody] List<CarritoItem> items)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return Json(new { success = false, message = "Usuario no autenticado" });
            }

            try
            {
                // Aquí iría la lógica para procesar la orden
                // Por ahora solo devolvemos un mensaje de éxito
                return Json(new { success = true, message = "Orden procesada correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // Clase para recibir los items del carrito
    public class CarritoItem
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
}
