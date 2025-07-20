using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TuneCastAPIConsumer;
using TuneCastModelo;

namespace TuneCast.MVC.Controllers
{
    [Authorize]
    public class ArtistaController : Controller
    {
        // Acción para gestionar las canciones del artista
        public IActionResult ManageSongs()
        {
            // Obtener el usuario actual basado en el email autenticado
            var usuario = Crud<Usuario>.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);

            if (usuario == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Obtener la suscripción del usuario
            var suscripcion = Crud<Suscripcion>.GetAll().FirstOrDefault(s => s.UsuarioId == usuario.Id);

            // Verificar si la suscripción existe y si el usuario tiene el plan adecuado (Personal, Familiar o Empresarial)
            if (suscripcion != null && (suscripcion.Plan.Nombre == "Personal" || suscripcion.Plan.Nombre == "Familiar" || suscripcion.Plan.Nombre == "Empresarial"))
            {
                // Obtener las canciones subidas por el artista (usuario)
                var canciones = Crud<Cancion>.GetAll().Where(c => c.Id == usuario.Id).ToList();
                return View(canciones);
            }

            TempData["Error"] = "Necesitas un plan Personal o superior para gestionar canciones.";
            return RedirectToAction("Index", "Home");
        }

        // Acción para crear (subir) una nueva canción
        [HttpGet]
        public IActionResult CreateSong()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSong(IFormFile archivo, Cancion cancion)
        {
            // Obtener el usuario actual basado en el email autenticado
            var usuario = Crud<Usuario>.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);

            if (usuario == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Obtener la suscripción del usuario
            var suscripcion = Crud<Suscripcion>.GetAll().FirstOrDefault(s => s.UsuarioId == usuario.Id);

            // Verificar si el usuario tiene el plan adecuado (Personal, Familiar o Empresarial)
            if (suscripcion == null || !(suscripcion.Plan.Nombre == "Personal" || suscripcion.Plan.Nombre == "Familiar" || suscripcion.Plan.Nombre == "Empresarial"))
            {
                TempData["Error"] = "Solo los usuarios con plan Personal o superior pueden subir canciones.";
                return RedirectToAction("Index", "Home");
            }

            if (archivo != null && archivo.Length > 0)
            {
                // Validar tipo de archivo
                var extensionesPermitidas = new[] { ".mp3", ".wav", ".ogg" };
                var extension = Path.GetExtension(archivo.FileName).ToLower();
                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("", "Solo se permiten archivos MP3, WAV o OGG.");
                    return View();
                }

                var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "canciones", archivo.FileName);
                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                cancion.RutaArchivo = "/canciones/" + archivo.FileName;
                cancion.Id = usuario.Id; // Asociar la canción al artista (usuario)

                // Guardar la canción
                await Crud<Cancion>.Create(cancion);
                return RedirectToAction(nameof(ManageSongs));
            }

            ModelState.AddModelError("", "El archivo no es válido.");
            return View();
        }

        // Acción para editar una canción existente
        [HttpGet]
        public IActionResult EditSong(int id)
        {
            var cancion = Crud<Cancion>.GetById(id);
            var usuario = Crud<Usuario>.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);

            if (cancion == null || cancion.Id != usuario.Id)
            {
                return NotFound();
            }

            return View(cancion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSong(int id, Cancion cancion, IFormFile imagen)
        {
            // Obtener el usuario actual basado en el email autenticado
            var usuario = Crud<Usuario>.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);
            if (usuario == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Obtener la suscripción del usuario
            var suscripcion = Crud<Suscripcion>.GetAll().FirstOrDefault(s => s.UsuarioId == usuario.Id);
            if (suscripcion == null || !(suscripcion.Plan.Nombre == "Personal" || suscripcion.Plan.Nombre == "Familiar" || suscripcion.Plan.Nombre == "Empresarial"))
            {
                TempData["Error"] = "Solo los usuarios con plan Personal o superior pueden editar canciones.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener la canción que se quiere editar
            var cancionExistente = Crud<Cancion>.GetById(id);
            if (cancionExistente == null || cancionExistente.Id != usuario.Id)
            {
                return NotFound();
            }

            // Si se recibe una imagen de portada
            if (imagen != null && imagen.Length > 0)
            {
                // Validar el formato de la imagen
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(imagen.FileName).ToLower();

                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("", "Formato de imagen no permitido.");
                    return View(cancion);
                }

                // Guardar la imagen en el servidor
                var nombreArchivo = $"portada_{id}{extension}";
                var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "portadas", nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                // Aquí no agregamos la propiedad Portada al modelo Cancion, solo usamos la ruta temporalmente
                // La ruta se usará solo durante este método, sin almacenarla en la base de datos.
                var rutaPortada = "/portadas/" + nombreArchivo;

                // Guardamos la canción sin modificar el modelo, pero usando la ruta de la portada
                //cancionExistente.rutaPortada = rutaPortada;
            }

            // Actualizar la canción con la nueva portada (sin modificar el modelo de Cancion)
            Crud<Cancion>.Update(id, cancionExistente);

            // Redirigir a la página de gestión de canciones
            return RedirectToAction(nameof(ManageSongs));
        }



        // Acción para eliminar una canción
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dvoi(int id)
        {
            var usuario = Crud<Usuario>.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);
            if (usuario == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var suscripcion = Crud<Suscripcion>.GetAll().FirstOrDefault(s => s.UsuarioId == usuario.Id);
            if (suscripcion == null || !(suscripcion.Plan.Nombre == "Personal" || suscripcion.Plan.Nombre == "Familiar" || suscripcion.Plan.Nombre == "Empresarial"))
            {
                TempData["Error"] = "Solo los usuarios con plan Personal o superior pueden eliminar canciones.";
                return RedirectToAction("Index", "Home");
            }

            var cancion = Crud<Cancion>.GetById(id);
            if (cancion == null || cancion.Id != usuario.Id)
            {
                return NotFound();
            }

            Crud<Cancion>.Delete(id);
            return RedirectToAction(nameof(ManageSongs));
        }
    }
}




