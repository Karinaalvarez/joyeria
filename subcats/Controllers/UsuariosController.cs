using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using subcats.customClass;
using subcats.Models;
using System;

namespace subcats.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public UsuariosController()
        {
            _usuarioService = new UsuarioService();
        }

        // GET: Usuarios/Index
        public IActionResult Index()
        {
            // Verificar si el usuario está autenticado y es administrador
            bool isAuthenticated = AuthController.IsAuthenticated(HttpContext);
            bool isAdmin = AuthController.IsAdmin(HttpContext);
            
            // Se ha eliminado el mensaje de diagnóstico que mostraba información de autenticación
            
            if (!isAuthenticated || !isAdmin)
            {
                return RedirectToAction("Login", "Auth");
            }
            
            var usuarios = _usuarioService.ObtenerTodosUsuarios();
            return View(usuarios);
        }

        // GET: Usuarios/Crear
        public IActionResult Crear()
        {
            // Verificar si el usuario está autenticado y es administrador
            if (!AuthController.IsAuthenticated(HttpContext) || !AuthController.IsAdmin(HttpContext))
            {
                return RedirectToAction("Login", "Auth");
            }
            
            return View();
        }

        // POST: Usuarios/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Usuario usuario)
        {
            // Verificar si el usuario está autenticado y es administrador
            if (!AuthController.IsAuthenticated(HttpContext) || !AuthController.IsAdmin(HttpContext))
            {
                return RedirectToAction("Login", "Auth");
            }
            
            if (ModelState.IsValid)
            {
                if (_usuarioService.CrearUsuario(usuario))
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al crear el usuario");
            }
            return View(usuario);
        }

        // GET: Usuarios/Editar/5
        public IActionResult Editar(int id)
        {
            // Verificar si el usuario está autenticado y es administrador
            if (!AuthController.IsAuthenticated(HttpContext) || !AuthController.IsAdmin(HttpContext))
            {
                return RedirectToAction("Login", "Auth");
            }
            
            var usuarios = _usuarioService.ObtenerTodosUsuarios();
            var usuario = usuarios.Find(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }
            
            // Convertir el usuario a EditarUsuarioViewModel
            var viewModel = new EditarUsuarioViewModel
            {
                Id = usuario.Id,
                Username = usuario.Username,
                Password = "", // No mostramos la contraseña actual por seguridad
                Role = usuario.Role
            };
            
            return View(viewModel);
        }

        // POST: Usuarios/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, EditarUsuarioViewModel viewModel)
        {
            // Verificar si el usuario está autenticado y es administrador
            if (!AuthController.IsAuthenticated(HttpContext) || !AuthController.IsAdmin(HttpContext))
            {
                return RedirectToAction("Login", "Auth");
            }
            
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            // Eliminar cualquier error de validación para el campo Password
            ModelState.Remove("Password");

            // Validar manualmente el modelo sin considerar la contraseña
            if (string.IsNullOrEmpty(viewModel.Username))
            {
                ModelState.AddModelError("Username", "El nombre de usuario es obligatorio");
            }
            if (string.IsNullOrEmpty(viewModel.Role))
            {
                ModelState.AddModelError("Role", "El rol es obligatorio");
            }

            if (ModelState.IsValid)
            {
                // Obtener el usuario actual para preservar la contraseña si no se proporciona una nueva
                var usuarioActual = _usuarioService.ObtenerTodosUsuarios().Find(u => u.Id == id);
                if (usuarioActual == null)
                {
                    return NotFound();
                }

                // Crear un objeto Usuario con los datos actualizados
                var usuarioActualizado = new Usuario
                {
                    Id = viewModel.Id,
                    Username = viewModel.Username,
                    Role = viewModel.Role,
                    // Si no se proporciona una nueva contraseña, mantener la actual
                    Password = string.IsNullOrEmpty(viewModel.Password) ? usuarioActual.Password : viewModel.Password
                };

                if (_usuarioService.ActualizarUsuario(usuarioActualizado))
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al actualizar el usuario");
            }
            return View(viewModel);
        }

        // POST: Usuarios/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            // Verificar si el usuario está autenticado y es administrador
            if (!AuthController.IsAuthenticated(HttpContext) || !AuthController.IsAdmin(HttpContext))
            {
                return RedirectToAction("Login", "Auth");
            }
            
            if (_usuarioService.EliminarUsuario(id))
            {
                return RedirectToAction(nameof(Index));
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}