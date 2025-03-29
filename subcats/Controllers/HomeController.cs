using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using subcats.Models;
using subcats.customClass;
using System.Diagnostics;

namespace subcats.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TematicaService _tematicaService;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _tematicaService = new TematicaService();
        }

        public IActionResult Index()
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Obtener la temática activa para mostrarla en la vista
            var tematicaActiva = _tematicaService.ObtenerTematicaActiva();
            if (tematicaActiva != null)
            {
                ViewBag.TematicaActiva = tematicaActiva;
            }

            // Mostrar la vista de inicio con la temática
            return View("Inicio");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
