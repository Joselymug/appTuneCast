
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneCastAPIConsumer;  // Para usar el Crud<T>
using TuneCastModelo;  // Para usar el modelo Cancion

namespace TuneCast.MVC.Controllers
{
   
    public class ReproduccionController : Controller
    {
        // GET: ReproduccionController/Reproducir
        public IActionResult Reproducir(int id)
        {
            var cancion = Crud<Cancion>.GetById(id);  // Obtener la canción por su ID
            if (cancion == null)
            {
                return NotFound();
            }
            // Verificar el rol del usuario
            var usuario = Crud<Usuario>.GetAll().FirstOrDefault(u => u.Email == User.Identity.Name);
            var esCliente = usuario != null && usuario.Rol == "Cliente";

            // Obtener un anuncio aleatorio si el usuario es Cliente
            Anuncio anuncioAleatorio = null;
            if (esCliente)
            {
                var anuncios = Crud<Anuncio>.GetAll();
                if (anuncios.Any())
                {
                    var random = new Random();
                    anuncioAleatorio = anuncios.ElementAt(random.Next(anuncios.Count()));
                }
            }

            // Pasamos la información del anuncio aleatorio a la vista
            ViewBag.EsCliente = esCliente;
            ViewBag.AnuncioAleatorio = anuncioAleatorio;

            return View(cancion);  // Pasar la canción a la vista para reproducir
        }
    }
}
