using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TuneCast.MVC.Models;

namespace TuneCast.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)  // Verifica si el usuario no est� autenticado
            {
                return RedirectToAction("Login", "Account");  // Redirige a la p�gina de login
            }

            return View();
        
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
