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