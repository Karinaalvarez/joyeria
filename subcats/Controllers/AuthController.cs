using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using subcats.customClass;
using subcats.Models;
using subcats.dto;
using System;

namespace subcats.Controllers
{
    public class AuthController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public AuthController()
        {
            _usuarioService = new UsuarioService();
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            // Si ya está autenticado, redirigir al Home
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        public IActionResult Login(LoginDTO loginDto)
        {
            if (ModelState.IsValid)
            {
                var usuario = _usuarioService.ValidarUsuario(loginDto.Username, loginDto.Password);
                if (usuario != null)
                {
                    // Guardar información del usuario en la sesión
                    HttpContext.Session.SetString("UserId", usuario.Id.ToString());
                    HttpContext.Session.SetString("Username", usuario.Username);
                    HttpContext.Session.SetString("Role", usuario.Role);

                    // Se ha eliminado el mensaje temporal de diagnóstico

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Nombre de usuario o contraseña incorrectos");
            }
            return View(loginDto);
        }

        // GET: Auth/Register
        public IActionResult Register()
        {
            // Si ya está autenticado, redirigir al Home
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterDTO registerDto)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el usuario ya existe
                var usuarioExistente = _usuarioService.ObtenerUsuarioPorUsername(registerDto.Username);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Username", "Este nombre de usuario ya está en uso");
                    return View(registerDto);
                }

                // Crear nuevo usuario con rol "User"
                var nuevoUsuario = new Usuario
                {
                    Username = registerDto.Username,
                    Password = registerDto.Password,
                    Role = "User" // Por defecto, todos los usuarios registrados tendrán rol "User"
                };

                bool resultado = _usuarioService.CrearUsuario(nuevoUsuario);
                if (resultado)
                {
                    // Redirigir al login con mensaje de éxito
                    TempData["SuccessMessage"] = "Registro exitoso. Ahora puedes iniciar sesión.";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError("", "Error al registrar el usuario");
                }
            }
            return View(registerDto);
        }

        // GET: Auth/Logout
        public IActionResult Logout()
        {
            // Eliminar la sesión
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Método para verificar si el usuario está autenticado
        public static bool IsAuthenticated(HttpContext context)
        {
            return context.Session.GetString("UserId") != null;
        }

        // Método para verificar si el usuario es administrador
        public static bool IsAdmin(HttpContext context)
        {
            var role = context.Session.GetString("Role");
            return role == "Admin";
        }
    }
}