using Microsoft.AspNetCore.Mvc;
using subcats.customClass;
using subcats.dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace subcats.Controllers
{
    public class ReportesController : Controller
    {
        private readonly VentasDao _ventasDao;
        private const int ItemsPorPagina = 10;

        public ReportesController()
        {
            _ventasDao = new VentasDao();
        }

        public IActionResult Index(string searchTerm = "", int pagina = 1)
        {
            // Verificar si el usuario ha iniciado sesión
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Solo permitir acceso a administradores
            if (role != "Admin")
            {
                TempData["ErrorMessage"] = "No tienes permisos para acceder a esta sección.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener todas las ventas
            var ventas = _ventasDao.ObtenerTodasLasOrdenes();
            
            // Filtrar por término de búsqueda si se proporciona
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                ventas = ventas.Where(v => 
                    (v.NombreCliente + " " + v.ApellidoCliente).ToLower().Contains(searchTerm) ||
                    v.NombreCliente.ToLower().Contains(searchTerm) ||
                    v.ApellidoCliente.ToLower().Contains(searchTerm)
                ).ToList();
                
                // Guardar el término de búsqueda para mostrarlo en la vista
                ViewBag.SearchTerm = searchTerm;
            }

            // Calcular la paginación
            var totalItems = ventas.Count;
            var totalPaginas = (int)Math.Ceiling(totalItems / (double)ItemsPorPagina);
            
            // Asegurar que la página actual es válida
            pagina = Math.Max(1, Math.Min(pagina, totalPaginas));
            
            // Obtener solo los items de la página actual
            var ventasPaginadas = ventas
                .Skip((pagina - 1) * ItemsPorPagina)
                .Take(ItemsPorPagina)
                .ToList();

            // Configurar ViewBag para la paginación
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalItems = totalItems;
            ViewBag.ItemsPorPagina = ItemsPorPagina;
            
            return View(ventasPaginadas);
        }

        public IActionResult Detalles(int id)
        {
            // Verificar si el usuario ha iniciado sesión
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Solo permitir acceso a administradores
            if (role != "Admin")
            {
                TempData["ErrorMessage"] = "No tienes permisos para acceder a esta sección.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener los detalles de la orden
            var detalles = _ventasDao.ObtenerDetallesOrden(id);
            ViewBag.OrdenId = id;
            
            // Obtener la información de la orden
            var orden = _ventasDao.ObtenerOrdenPorId(id);
            ViewBag.Orden = orden;
            
            // Obtener la información del cliente
            if (orden != null)
            {
                var cliente = _ventasDao.ObtenerClientePorId(orden.Id_cliente);
                ViewBag.Cliente = cliente;
            }
            
            return View(detalles);
        }
        
        public IActionResult GenerarPDF(int id)
        {
            // Verificar si el usuario ha iniciado sesión
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Solo permitir acceso a administradores
            if (role != "Admin")
            {
                TempData["ErrorMessage"] = "No tienes permisos para acceder a esta sección.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener los detalles de la orden
            var detalles = _ventasDao.ObtenerDetallesOrden(id);
            ViewBag.OrdenId = id;
            
            // Obtener la información de la orden
            var orden = _ventasDao.ObtenerOrdenPorId(id);
            ViewBag.Orden = orden;
            
            // Obtener la información del cliente
            if (orden != null)
            {
                var cliente = _ventasDao.ObtenerClientePorId(orden.Id_cliente);
                ViewBag.Cliente = cliente;
            }
            
            // Establecer la vista para renderizar sin layout
            return View("ImprimirDetalles", detalles);
        }
        
        public IActionResult ImprimirListado(string searchTerm = "")
        {
            // Verificar si el usuario ha iniciado sesión
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Solo permitir acceso a administradores
            if (role != "Admin")
            {
                TempData["ErrorMessage"] = "No tienes permisos para acceder a esta sección.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener todas las ventas
            var ventas = _ventasDao.ObtenerTodasLasOrdenes();
            
            // Filtrar por término de búsqueda si se proporciona
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                ventas = ventas.Where(v => 
                    (v.NombreCliente + " " + v.ApellidoCliente).ToLower().Contains(searchTerm) ||
                    v.NombreCliente.ToLower().Contains(searchTerm) ||
                    v.ApellidoCliente.ToLower().Contains(searchTerm)
                ).ToList();
                
                // Guardar el término de búsqueda para mostrarlo en la vista
                ViewBag.SearchTerm = searchTerm;
            }
            
            return View(ventas);
        }
    }
}
