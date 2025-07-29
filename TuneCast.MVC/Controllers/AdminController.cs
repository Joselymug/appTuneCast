using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TuneCast.MVC.Controllers;
using TuneCastAPIConsumer;
using TuneCastModelo;
using TuneCast.MVC.Models; 

namespace TuneCast.MVC.Controllers
{
    // Aseguramos que solo el Admin puede acceder a estas acciones.
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly CancionesController _cancionesController;
        private readonly SubscripcionesController _suscripcionesController;
        public AdminController()
        {
            // Inicializamos el controlador Canciones
            _cancionesController = new CancionesController();
        }
        public ActionResult Index(string rol)
        {
            var data = Crud<Usuario>.GetByRol(rol);
            return View(data);
        }
        // Acción para eliminar canciones
        public IActionResult ManageSongs()
        {
            var canciones = _cancionesController.Index();
            return View(canciones);

        }

        // Acción para gestionar planes de suscripción
        public IActionResult ManageSubscriptions()
        {
            var suscripciones = _suscripcionesController.Index();
            return View(suscripciones);
        }

        // Acción para ver los reportes
        public IActionResult ViewReports()
        {
            var canciones = Crud<Cancion>.GetAll(); // Cambiar por el método real para obtener las canciones
            var usuarios = Crud<Usuario>.GetAll(); // Cambiar por el método real para obtener los usuarios
            var pagos = Crud<Pago>.GetAll(); // Cambiar por el método real para obtener los pagos
            var playlists = Crud<Playlist>.GetAll(); // Cambiar por el método real para obtener las playlists
            //var artistas = Crud<Artista>.GetAll(); // Cambiar por el método real para obtener los artistas
            var planes = Crud<Plan>.GetAll(); // Cambiar por el método real para obtener los planes

            // Crear el ViewModel con todas las listas
            var reportesViewModel = new ReportesViewModel
            {
                Canciones = canciones.ToList(),
                Usuarios = usuarios.ToList(),
                Pagos = pagos.ToList(),
                Playlists = playlists.ToList(),
                //Artistas = artistas.ToList(),
                Planes = planes.ToList()
            };

            // Pasar el ViewModel a la vista
            return View(reportesViewModel);
        }
    }
}
