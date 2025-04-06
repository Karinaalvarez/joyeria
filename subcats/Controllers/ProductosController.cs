using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using subcats.customClass;
using subcats.dto;
using subcats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace subcats.Controllers
{
    public class ProductosController : Controller
    {
        private readonly Dao _db;
        private readonly CategoriaService _categoriaService;
        private readonly TematicaService _tematicaService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor que maneja la inyección de dependencias
        public ProductosController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _db = new Dao();
            _categoriaService = new CategoriaService();
            _tematicaService = new TematicaService();
        }

        // GET: Productos
        public IActionResult Index(string searchString)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var productos = _db.GetAllProductos();
                
                // Guardar el término de búsqueda actual para mostrarlo en el campo de búsqueda
                ViewData["CurrentFilter"] = searchString;
                
                // Filtrar productos por nombre si se proporciona un término de búsqueda
                if (!string.IsNullOrEmpty(searchString))
                {
                    productos = productos.Where(p => p.Nombre.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                // Verificar el rol del usuario
                var role = HttpContext.Session.GetString("Role");
                
                // Si es un usuario normal, mostrar una vista simplificada
                if (role != "Admin")
                {
                    // Obtener la temática activa para mostrarla en la vista
                    var tematicaActiva = _tematicaService.ObtenerTematicaActiva();
                    if (tematicaActiva != null)
                    {
                        ViewBag.TematicaActiva = tematicaActiva;
                    }
                    
                    return View("Catalogo", productos);
                }
                
                // Si es administrador, mostrar la vista completa
                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al obtener los productos: " + ex.Message;
                return View(new List<Producto>());
            }
        }

        // GET: Productos/Details/5
        public IActionResult Details(int id)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var producto = _db.GetProducto(id.ToString());
                if (producto == null || producto.Id_producto == 0)
                {
                    return NotFound();
                }

                // Si el producto tiene categoría, cargar su nombre
                if (producto.CategoriaId.HasValue)
                {
                    var categoria = _categoriaService.ObtenerCategoria(producto.CategoriaId.Value);
                    ViewBag.NombreCategoria = categoria?.Nombre ?? "Categoría no encontrada";
                }

                // Verificar el rol del usuario
                var role = HttpContext.Session.GetString("Role");
                
                // Si es un usuario normal, mostrar una vista simplificada
                if (role != "Admin")
                {
                    return View("UserDetails", producto);
                }
                
                // Si es administrador, mostrar la vista completa
                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el producto: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Cargar las categorías y proveedores para el select
            CargarCategorias();
            CargarProveedores();

            // Inicializar producto con valores por defecto
            var producto = new Producto
            {
                Precio = 0.01m,
                Impuesto = 0,
                Stock = 0,
                Fecha_creacion = DateTime.Now
            };

            return View(producto);
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Excluir Imagen de la validación del modelo
                ModelState.Remove("Imagen");

                // Cargar categorías para el select (en caso de error)
                CargarCategorias();

                // Imprimir todos los valores recibidos para depuración
                Console.WriteLine($"Valores recibidos: Nombre={producto.Nombre}, Precio={producto.Precio}, Impuesto={producto.Impuesto}, Stock={producto.Stock}");
                
                // Inicializar valores por defecto para evitar errores de validación
                if (producto.Precio <= 0)
                {
                    producto.Precio = 0.01m;
                }
                
                if (producto.Impuesto < 0)
                {
                    producto.Impuesto = 0;
                }
                
                if (producto.Stock < 0)
                {
                    producto.Stock = 0;
                }

                // Manejar la carga de la imagen
                if (producto.ImagenFile != null)
                {
                    Console.WriteLine($"Procesando archivo: {producto.ImagenFile.FileName}, tamaño: {producto.ImagenFile.Length} bytes");
                    
                    if (producto.ImagenFile.Length > 10 * 1024 * 1024) // 10MB máximo
                    {
                        ModelState.AddModelError("ImagenFile", "La imagen es demasiado grande. El tamaño máximo es 10MB.");
                        TempData["ErrorMessage"] = "La imagen es demasiado grande. El tamaño máximo es 10MB.";
                        return View(producto);
                    }

                    string extension = Path.GetExtension(producto.ImagenFile.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                    {
                        ModelState.AddModelError("ImagenFile", "Solo se permiten archivos de imagen con formato JPG, JPEG, PNG o GIF.");
                        TempData["ErrorMessage"] = "Solo se permiten archivos de imagen con formato JPG, JPEG, PNG o GIF.";
                        return View(producto);
                    }

                    // Convertir la imagen a bytes[]
                    try
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await producto.ImagenFile.CopyToAsync(memoryStream);
                            producto.Imagen = memoryStream.ToArray();
                            Console.WriteLine($"Imagen convertida a bytes: {producto.Imagen.Length} bytes");
                            
                            // Verificar que la imagen se convirtió correctamente
                            if (producto.Imagen == null || producto.Imagen.Length == 0)
                            {
                                ModelState.AddModelError("ImagenFile", "No se pudo leer la imagen. Intente con otra imagen.");
                                TempData["ErrorMessage"] = "No se pudo leer la imagen. Intente con otra imagen.";
                                return View(producto);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al convertir la imagen: {ex.Message}");
                        ModelState.AddModelError("ImagenFile", "Error al procesar la imagen: " + ex.Message);
                        TempData["ErrorMessage"] = "Error al procesar la imagen.";
                        return View(producto);
                    }
                }
                else
                {
                    Console.WriteLine("No se ha proporcionado un archivo de imagen");
                    // La imagen es opcional, no mostrar error
                    producto.Imagen = null;
                }

                // Mostrar todos los errores del ModelState para depuración
                ImprimirErroresModelState();

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Modelo inválido. Errores encontrados.");
                    // Agregar mensaje de error general
                    TempData["ErrorMessage"] = "Hay errores en el formulario. Por favor revise los campos marcados.";
                    return View(producto);
                }

                Console.WriteLine("Modelo válido. Intentando guardar el producto...");
                
                try
                {
                    int idProducto = _db.InsertarProducto(producto);
                    if (idProducto > 0)
                    {
                        Console.WriteLine($"Producto creado exitosamente con ID: {idProducto}");
                    TempData["SuccessMessage"] = "Producto creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        Console.WriteLine("No se pudo crear el producto en la base de datos.");
                        TempData["ErrorMessage"] = "No se pudo crear el producto en la base de datos.";
                        return View(producto);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al insertar el producto en la base de datos: {ex.Message}");
                    TempData["ErrorMessage"] = $"Error al guardar: {ex.Message}";
                return View(producto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                TempData["ErrorMessage"] = $"Error al crear el producto: {ex.Message}";
                return View(producto);
            }
        }

        // GET: Productos/Edit/5
        public IActionResult Edit(int id)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var producto = _db.GetProducto(id.ToString());
                if (producto == null || producto.Id_producto == 0)
                {
                    return NotFound();
                }

                // Cargar las categorías y proveedores para el select
                CargarCategorias();
                CargarProveedores();

                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el producto: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Excluir Imagen e ImagenFile de la validación del modelo
                ModelState.Remove("Imagen");
                ModelState.Remove("ImagenFile");
                
                // Ignorar todos los errores de validación relacionados con ImagenFile
                foreach (var key in ModelState.Keys.ToList())
                {
                    if (key.Contains("ImagenFile"))
                    {
                        ModelState.Remove(key);
                    }
                }

                // Cargar las categorías para el select (en caso de error)
                CargarCategorias();
                CargarProveedores();

                if (id != producto.Id_producto)
                {
                    return NotFound();
                }

                // Recuperamos el producto actual para mantener la imagen si no se carga una nueva
                var productoActual = _db.GetProducto(id.ToString());
                if (productoActual == null || productoActual.Id_producto == 0)
                {
                    TempData["ErrorMessage"] = "No se encontró el producto a editar.";
                    return RedirectToAction(nameof(Index));
                }

                // Manejar la carga de imagen, si hay una nueva
                if (producto.ImagenFile != null && producto.ImagenFile.Length > 0)
                {
                    try
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await producto.ImagenFile.CopyToAsync(memoryStream);
                            producto.Imagen = memoryStream.ToArray();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar la imagen: {ex.Message}");
                        // No interrumpimos el flujo, simplemente mantenemos la imagen anterior
                        producto.Imagen = productoActual.Imagen;
                    }
                }
                else
                {
                    // Si no se seleccionó nueva imagen, mantener la anterior
                    producto.Imagen = productoActual.Imagen;
                }

                try
                {
                    bool actualizado = _db.ActualizarProducto(producto);
                    if (actualizado)
                    {
                        TempData["SuccessMessage"] = "Producto actualizado exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No se pudo actualizar el producto. Verifique los datos e intente nuevamente.";
                        return View(producto);
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al guardar: {ex.Message}";
                    return View(producto);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error general: {ex.Message}";
                return View(producto);
            }
        }

        // GET: Productos/Delete/5
        public IActionResult Delete(int id)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var producto = _db.GetProducto(id.ToString());
                if (producto == null || producto.Id_producto == 0)
                {
                    return NotFound();
                }

                // Si el producto tiene categoría, cargar su nombre
                if (producto.CategoriaId.HasValue)
                {
                    var categoria = _categoriaService.ObtenerCategoria(producto.CategoriaId.Value);
                    ViewBag.NombreCategoria = categoria?.Nombre ?? "Categoría no encontrada";
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el producto: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                _db.EliminarProducto(id.ToString());
                TempData["SuccessMessage"] = "Producto eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar el producto: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Productos/GetImage/5
        public IActionResult GetImage(int id)
        {
            try
            {
                var producto = _db.GetProducto(id.ToString());
                if (producto?.Imagen == null || producto.Imagen.Length == 0)
                {
                    // Devolver una imagen por defecto o un error 404
                    return NotFound();
                }

                // Determinar el tipo de contenido basado en los bytes de la imagen
                string contentType = DeterminarTipoContenido(producto.Imagen);
                
                // Devolver la imagen
                return File(producto.Imagen, contentType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener imagen: {ex.Message}");
                return NotFound();
            }
        }

        // Método auxiliar para determinar el tipo de contenido basado en los bytes
        private string DeterminarTipoContenido(byte[] bytes)
        {
            // Verificar los primeros bytes del archivo para determinar el tipo
            if (bytes.Length >= 2)
            {
                if (bytes[0] == 0xFF && bytes[1] == 0xD8) // JPEG
                    return "image/jpeg";
                if (bytes[0] == 0x89 && bytes[1] == 0x50) // PNG
                    return "image/png";
                if (bytes[0] == 0x47 && bytes[1] == 0x49) // GIF
                    return "image/gif";
                if (bytes[0] == 0x42 && bytes[1] == 0x4D) // BMP
                    return "image/bmp";
            }

            // Por defecto, devolver como imagen JPEG
            return "image/jpeg";
        }

        // Método privado para cargar las categorías en ViewBag
        private void CargarCategorias()
        {
            try
            {
                var categorias = _categoriaService.ObtenerTodasCategorias();
                ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre");
            }
            catch (Exception)
            {
                ViewBag.Categorias = new SelectList(new List<Categoria>(), "Id", "Nombre");
            }
        }

        private void CargarProveedores()
        {
            try
            {
                var proveedores = _db.GetAllProveedores();
                ViewBag.Proveedores = new SelectList(proveedores, "Id_proveedor", "Nombre");
            }
            catch (Exception)
            {
                ViewBag.Proveedores = new SelectList(new List<Proveedor>(), "Id_proveedor", "Nombre");
            }
        }

        // Método para imprimir todos los errores del ModelState
        private void ImprimirErroresModelState()
        {
            Console.WriteLine($"Estado del ModelState: IsValid={ModelState.IsValid}, ErrorCount={ModelState.ErrorCount}");
            
            foreach (var key in ModelState.Keys)
            {
                var modelStateEntry = ModelState[key];
                if (modelStateEntry.Errors.Count > 0)
                {
                    Console.WriteLine($"Errores para el campo '{key}':");
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Console.WriteLine($"  - {error.ErrorMessage}");
                        if (error.Exception != null)
                        {
                            Console.WriteLine($"  - Exception: {error.Exception.Message}");
                        }
                    }
                }
            }
        }

        // GET: Productos/ObtenerCategorias
        [HttpGet]
        public IActionResult ObtenerCategorias()
        {
            try
            {
                var categorias = _categoriaService.ObtenerTodasCategorias();
                return Json(categorias);
            }
            catch (Exception ex)
            {
                return Json(new List<Categoria>());
            }
        }

        // GET: Productos/Categoria/5
        public IActionResult Categoria(int id)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                List<Producto> productos;
                
                if (id == 0)
                {
                    // Si id es 0, mostrar todos los productos
                    productos = _db.GetAllProductos();
                }
                else
                {
                    // Obtener productos por categoría
                    productos = _db.GetProductosPorCategoria(id);
                }
                
                // Obtener el nombre de la categoría
                string nombreCategoria = "Todos los Productos";
                if (id > 0)
                {
                    var categoria = _categoriaService.ObtenerCategoria(id);
                    if (categoria != null)
                    {
                        nombreCategoria = categoria.Nombre;
                        Console.WriteLine($"Mostrando categoría: {nombreCategoria} (ID: {id})");
                    }
                }
                
                ViewBag.CategoriaId = id;
                ViewBag.NombreCategoria = nombreCategoria;
                
                // Verificar el rol del usuario
                var role = HttpContext.Session.GetString("Role");
                Console.WriteLine($"Rol del usuario: {role}");
                
                // Si es un usuario normal, mostrar una vista simplificada
                if (role != "Admin")
                {
                    // Obtener la temática activa para mostrarla en la vista
                    var tematicaActiva = _tematicaService.ObtenerTematicaActiva();
                    if (tematicaActiva != null)
                    {
                        ViewBag.TematicaActiva = tematicaActiva;
                    }
                    
                    Console.WriteLine("Mostrando vista 'Catalogo' para usuario normal");
                    return View("Catalogo", productos);
                }
                
                // Si es administrador, mostrar la vista completa
                Console.WriteLine("Mostrando vista 'Index' para administrador");
                return View("Index", productos);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al obtener los productos: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        
        // GET: Productos/DebuggingInfo
        public IActionResult DebuggingInfo()
        {
            // Verificar la existencia de las imágenes en la carpeta wwwroot/imagenes
            string wwwrootPath = _webHostEnvironment.WebRootPath;
            string imagePath = Path.Combine(wwwrootPath, "imagenes", "bebe.png");
            
            bool fileExists = System.IO.File.Exists(imagePath);
            long fileSize = fileExists ? new FileInfo(imagePath).Length : 0;
            
            Console.WriteLine($"Path de la imagen: {imagePath}");
            Console.WriteLine($"La imagen existe: {fileExists}");
            Console.WriteLine($"Tamaño de la imagen: {fileSize} bytes");
            
            // Mostrar rutas importantes
            Console.WriteLine($"WebRootPath: {_webHostEnvironment.WebRootPath}");
            Console.WriteLine($"ContentRootPath: {_webHostEnvironment.ContentRootPath}");
            
            // Devolver algo para la vista
            ViewBag.ImagePath = imagePath;
            ViewBag.FileExists = fileExists;
            ViewBag.FileSize = fileSize;
            
            return View();
        }
    }
}