using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TuneCastAPIConsumer;
using TuneCastModelo;

namespace TuneCast.MVC.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        // Acción para gestionar la reproducción de música (para cliente Free y Premium)
        public IActionResult PlayMusic()
        {
            var usuario = Crud<Usuario>.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);

            if (usuario == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Obtener la suscripción del usuario
            var suscripcion = Crud<Suscripcion>.GetAll().FirstOrDefault(s => s.UsuarioId == usuario.Id);

            if (suscripcion != null)
            {
                if (suscripcion.Plan.Nombre == "Free")
                {
                    // Acciones específicas para cliente Free
                    TempData["Mensaje"] = "Escucha música con anuncios y restricciones (sin adelantar o retroceder).";
                    return View("FreeMusic");  // Vista con restricciones de Free
                }
                else if (suscripcion.Plan.Nombre == "Premium")
                {
                    // Acciones específicas para cliente Premium
                    TempData["Mensaje"] = "Escucha música sin anuncios, con opción de adelantar/retroceder y descargar.";
                    return View("PremiumMusic");  // Vista sin anuncios, con funcionalidades avanzadas
                }
            }

            TempData["Error"] = "No se pudo determinar el plan del usuario.";
            return RedirectToAction("Index", "Home");
        }

        // Acción para escuchar música en modo Free
        public IActionResult FreeMusic()
        { 
            return View();
        }

        // Acción para escuchar música en modo Premium
        public IActionResult PremiumMusic()
        {
            // Aquí va la lógica para la vista de música para el cliente Premium
            // Mostrar canciones, sin anuncios y con funcionalidades avanzadas (adelantar, retroceder, descargar)
            return View();
        }

        // Acción para descargar canciones (solo para Premium)
        [HttpPost]
        public IActionResult DownloadMusic(int id)
        {
            var usuario = Crud<Usuario>.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);
            var suscripcion = Crud<Suscripcion>.GetAll().FirstOrDefault(s => s.UsuarioId == usuario.Id);

            if (suscripcion != null && suscripcion.Plan.Nombre == "Premium")
            {
                var cancion = Crud<Cancion>.GetById(id);
                if (cancion != null)
                {
                    // Lógica para la descarga de la canción
                    // Suponiendo que el archivo esté en /wwwroot/canciones/
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "canciones", cancion.RutaArchivo);
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "audio/mpeg", cancion.Titulo + ".mp3");
                }
                TempData["Error"] = "Canción no encontrada.";
                return RedirectToAction(nameof(PlayMusic));
            }

            TempData["Error"] = "Solo los usuarios Premium pueden descargar canciones.";
            return RedirectToAction(nameof(PlayMusic));
        }

        // Acción para crear playlists (solo para Premium)
        [HttpGet]
        public IActionResult CreatePlaylist()
        {
            var usuario = Crud<Usuario>.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);
            var suscripcion = Crud<Suscripcion>.GetAll().FirstOrDefault(s => s.UsuarioId == usuario.Id);

            if (suscripcion != null && suscripcion.Plan.Nombre == "Premium")
            {
                // Lógica para crear una playlist (solo para Premium)
                return View();
            }

            TempData["Error"] = "Solo los usuarios Premium pueden crear playlists.";
            return RedirectToAction(nameof(PlayMusic));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlaylist(string nombre)
        {
            var usuario = Crud<Usuario>.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);
            var suscripcion = Crud<Suscripcion>.GetAll().FirstOrDefault(s => s.UsuarioId == usuario.Id);

            if (suscripcion != null && suscripcion.Plan.Nombre == "Premium")
            {
                // Crear la playlist
                var playlist = new Playlist
                {
                    Nombre = nombre,
                    UsuarioId = usuario.Id
                };

                await Crud<Playlist>.Create(playlist);
                return RedirectToAction(nameof(PlayMusic));
            }

            TempData["Error"] = "Solo los usuarios Premium pueden crear playlists.";
            return RedirectToAction(nameof(PlayMusic));
        }
    }
}
