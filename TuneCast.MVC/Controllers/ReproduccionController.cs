//using Microsoft.AspNetCore.Mvc;
//using TuneCastAPIConsumer;
//using TuneCastModelo;

//namespace TuneCast.MVC.Controllers
//{
//    public class ReproduccionController : Controller
//    {
//        [HttpGet]
//        public IActionResult Index()
//        {
//            var canciones = Crud<Cancion>.GetAll();  
//            return View(canciones);  
//        }


//        // Accion para reproducir una canción seleccionada
//        [HttpGet]
//        public IActionResult Reproducir(int id)
//        {
//            var cancion = Crud<Cancion>.GetById(id);  
//            if (cancion == null)
//            {
//                return NotFound();
//            }

//            return View(cancion);  
//        }
//    }
//}
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

            return View(cancion);  // Pasar la canción a la vista para reproducir
        }
    }
}
