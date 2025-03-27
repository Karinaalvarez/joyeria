using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using subcats.dto;
using subcats.customClass;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace subcats.Controllers
{
    public class CarritoController : Controller
    {
        private readonly Dao _db;
        private readonly VentasDao _ventasDao;

        public CarritoController()
        {
            _db = new Dao();
            _ventasDao = new VentasDao();
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

            return View(new Cliente());
        }

        // POST: Carrito/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(Cliente cliente, string carritoJson)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Deserializar el carrito
                    var carritoItems = JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson);
                    
                    if (carritoItems == null || !carritoItems.Any())
                    {
                        TempData["ErrorMessage"] = "El carrito está vacío";
                        return View(cliente);
                    }

                    // Calcular el total
                    decimal total = carritoItems.Sum(item => item.Precio * item.Cantidad);

                    // Insertar cliente
                    int clienteId = _ventasDao.InsertarCliente(cliente);
                    if (clienteId <= 0)
                    {
                        TempData["ErrorMessage"] = "Error al registrar el cliente";
                        return View(cliente);
                    }

                    // Crear la orden
                    var orden = new Orden
                    {
                        Id_cliente = clienteId,
                        Total = total,
                        Estado = "Pendiente"
                    };

                    // Insertar orden
                    int ordenId = _ventasDao.InsertarOrden(orden);
                    if (ordenId <= 0)
                    {
                        TempData["ErrorMessage"] = "Error al crear la orden";
                        return View(cliente);
                    }

                    // Crear detalles de la orden
                    var detalles = new List<DetalleOrden>();
                    foreach (var item in carritoItems)
                    {
                        detalles.Add(new DetalleOrden
                        {
                            Id_orden = ordenId,
                            Id_producto = int.Parse(item.Id),
                            Cantidad = item.Cantidad,
                            Precio_Unitario = item.Precio,
                            Subtotal = item.Precio * item.Cantidad
                        });
                    }

                    // Insertar detalles
                    bool detallesInsertados = _ventasDao.InsertarDetallesOrden(detalles);
                    if (!detallesInsertados)
                    {
                        TempData["ErrorMessage"] = "Error al registrar los detalles de la orden";
                        return View(cliente);
                    }

                    // Guardar el ID de la orden en TempData para mostrarlo en la factura
                    TempData["OrdenId"] = ordenId;
                    
                    // Redirigir a la página de factura
                    return RedirectToAction("Factura", new { id = ordenId });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al procesar la orden: " + ex.Message;
                    return View(cliente);
                }
            }

            return View(cliente);
        }

        // GET: Carrito/Factura/5
        public IActionResult Factura(int id)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Obtener la orden completa con sus detalles
                var orden = _ventasDao.ObtenerOrdenCompleta(id);
                if (orden == null)
                {
                    TempData["ErrorMessage"] = "No se encontró la orden especificada";
                    return RedirectToAction("Index", "Home");
                }

                return View(orden);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la factura: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
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
                if (items == null || !items.Any())
                {
                    return Json(new { success = false, message = "El carrito está vacío" });
                }

                // Guardar los items en la sesión para usarlos en el checkout
                HttpContext.Session.SetString("CarritoItems", JsonConvert.SerializeObject(items));
                
                return Json(new { success = true, message = "Carrito procesado correctamente" });
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
