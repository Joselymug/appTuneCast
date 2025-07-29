using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TuneCast.MVC.Models;
using TuneCastAPIConsumer;
using TuneCastModelo;

namespace TuneCast.MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string q)
        {
            try
            {
                var todasLasCanciones = Crud<Cancion>.GetAll();
                var canciones = todasLasCanciones ?? new List<Cancion>();

                // Obtener playlists del usuario autenticado
                var usuarioId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(usuarioId))
                {
                    var todasLasPlaylists = Crud<Playlist>.GetAll();
                    var playlistsDelUsuario = todasLasPlaylists?.Where(p => p.UsuarioId.ToString() == usuarioId).ToList();
                    ViewBag.Playlists = playlistsDelUsuario;
                }

                // Si hay término de búsqueda, filtrar las canciones
                if (!string.IsNullOrWhiteSpace(q))
                {
                    var termino = q.ToLower().Trim();
                    canciones = canciones.Where(c =>
                        (c.Titulo != null && c.Titulo.ToLower().Contains(termino)) ||
                        (c.Artista != null && c.Artista.ToLower().Contains(termino)) ||
                        (c.Genero != null && c.Genero.ToLower().Contains(termino))
                    ).ToList();

                    // Pasar datos de búsqueda a la vista
                    ViewData["SearchQuery"] = q;
                    ViewData["SearchCount"] = canciones.Count;
                }

                return View(canciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar las canciones");
                return View(new List<Cancion>());
            }
        }

        private List<Usuario> GetUsuariosRol(string rol)
        {
            var data = Crud<Usuario>.GetAll();
            var artistas = data.Where(u => u.Rol == rol).ToList();

            return artistas;
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