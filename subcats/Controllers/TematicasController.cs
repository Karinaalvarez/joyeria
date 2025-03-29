using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using subcats.customClass;
using subcats.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace subcats.Controllers
{
    public class TematicasController : Controller
    {
        private readonly TematicaService _tematicaService;

        public TematicasController()
        {
            _tematicaService = new TematicaService();
        }

        // GET: Tematicas
        public IActionResult Index()
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Verificar si el usuario es administrador
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var tematicas = _tematicaService.ObtenerTematicas();
                return View(tematicas);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener las temáticas: " + ex.Message);
                return View(new System.Collections.Generic.List<Tematica>());
            }
        }

        // GET: Tematicas/Details/5
        public IActionResult Details(int id)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Verificar si el usuario es administrador
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var tematica = _tematicaService.ObtenerTematica(id);
                if (tematica == null)
                {
                    return NotFound();
                }

                return View(tematica);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener la temática: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Tematicas/Create
        public IActionResult Create()
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Verificar si el usuario es administrador
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Inicializar con valores por defecto
            var tematica = new Tematica
            {
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(30),
                Activa = false
            };

            return View(tematica);
        }

        // POST: Tematicas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tematica tematica)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Verificar si el usuario es administrador
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Excluir Imagen de la validación del modelo
                ModelState.Remove("Imagen");

                if (ModelState.IsValid)
                {
                    // Manejar la carga de la imagen
                    if (tematica.ImagenFile != null)
                    {
                        if (tematica.ImagenFile.Length > 10 * 1024 * 1024) // 10MB máximo
                        {
                            ModelState.AddModelError("ImagenFile", "La imagen es demasiado grande. El tamaño máximo es 10MB.");
                            return View(tematica);
                        }

                        string extension = Path.GetExtension(tematica.ImagenFile.FileName).ToLower();
                        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                        {
                            ModelState.AddModelError("ImagenFile", "Solo se permiten archivos de imagen con formato JPG, JPEG, PNG o GIF.");
                            return View(tematica);
                        }

                        // Convertir la imagen a bytes[]
                        using (var memoryStream = new MemoryStream())
                        {
                            await tematica.ImagenFile.CopyToAsync(memoryStream);
                            tematica.Imagen = memoryStream.ToArray();
                        }
                    }

                    // Guardar la temática
                    int id = _tematicaService.CrearTematica(tematica);
                    return RedirectToAction(nameof(Index));
                }

                return View(tematica);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al crear la temática: " + ex.Message);
                return View(tematica);
            }
        }

        // GET: Tematicas/Edit/5
        public IActionResult Edit(int id)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Verificar si el usuario es administrador
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var tematica = _tematicaService.ObtenerTematica(id);
                if (tematica == null)
                {
                    return NotFound();
                }

                // Convertir a EditarTematicaViewModel
                var viewModel = new EditarTematicaViewModel
                {
                    Id = tematica.Id,
                    Nombre = tematica.Nombre,
                    Descripcion = tematica.Descripcion,
                    FechaInicio = tematica.FechaInicio,
                    FechaFin = tematica.FechaFin,
                    Activa = tematica.Activa,
                    Imagen = tematica.Imagen
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener la temática: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Tematicas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditarTematicaViewModel viewModel)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Verificar si el usuario es administrador
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id != viewModel.Id)
            {
                return NotFound();
            }

            // Obtener la temática original para mantener la imagen si no se carga una nueva
            var tematicaOriginal = _tematicaService.ObtenerTematica(id);
            if (tematicaOriginal == null)
            {
                return NotFound();
            }

            // Crear un objeto Tematica para actualizar
            var tematica = new Tematica
            {
                Id = viewModel.Id,
                Nombre = viewModel.Nombre,
                Descripcion = viewModel.Descripcion,
                FechaInicio = viewModel.FechaInicio,
                FechaFin = viewModel.FechaFin,
                Activa = viewModel.Activa,
                Imagen = tematicaOriginal.Imagen // Por defecto, mantener la imagen original
            };

            try
            {
                // Manejar la carga de la imagen solo si se proporciona una nueva
                if (viewModel.ImagenFile != null && viewModel.ImagenFile.Length > 0)
                {
                    if (viewModel.ImagenFile.Length > 10 * 1024 * 1024) // 10MB máximo
                    {
                        ModelState.AddModelError("ImagenFile", "La imagen es demasiado grande. El tamaño máximo es 10MB.");
                        return View(viewModel);
                    }

                    string extension = Path.GetExtension(viewModel.ImagenFile.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                    {
                        ModelState.AddModelError("ImagenFile", "Solo se permiten archivos de imagen con formato JPG, JPEG, PNG o GIF.");
                        return View(viewModel);
                    }

                    // Convertir la imagen a bytes[]
                    using (var memoryStream = new MemoryStream())
                    {
                        await viewModel.ImagenFile.CopyToAsync(memoryStream);
                        tematica.Imagen = memoryStream.ToArray();
                    }
                }

                // Actualizar la temática
                bool resultado = _tematicaService.ActualizarTematica(tematica);
                if (resultado)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo actualizar la temática.");
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar la temática: " + ex.Message);
                ModelState.AddModelError("", "Error al actualizar la temática: " + ex.Message);
                return View(viewModel);
            }
        }

        // GET: Tematicas/Delete/5
        public IActionResult Delete(int id)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Verificar si el usuario es administrador
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var tematica = _tematicaService.ObtenerTematica(id);
                if (tematica == null)
                {
                    return NotFound();
                }

                return View(tematica);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener la temática: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Tematicas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Verificar si el usuario es administrador
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                bool resultado = _tematicaService.EliminarTematica(id);
                if (resultado)
                {
                    Console.WriteLine("Temática eliminada correctamente.");
                }
                else
                {
                    Console.WriteLine("No se pudo eliminar la temática.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar la temática: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Tematicas/GetImage/5
        public IActionResult GetImage(int id)
        {
            try
            {
                var tematica = _tematicaService.ObtenerTematica(id);
                if (tematica == null || tematica.Imagen == null)
                {
                    return NotFound();
                }

                return File(tematica.Imagen, "image/jpeg");
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
