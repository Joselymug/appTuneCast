using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TuneCastAPIConsumer;
using TuneCastModelo;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TuneCast.MVC.Controllers
{
    
    public class UsuariosController : Controller
    {
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var data = Crud<Usuario>.GetAll();  
            return View(data);
        }

        // GET: UsuariosController/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            var data = Crud<Usuario>.GetById(id);
            return View(data);
        }

        // GET: UsuariosController/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuariosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Usuario data)
        {
            try
            {
                Crud<Usuario>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: UsuariosController/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var data = Crud<Usuario>.GetById(id);
            return View(data);
        }

        // POST: UsuariosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Usuario data)
        {
            try
            {
                Crud<Usuario>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: UsuariosController/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)

        {
            var data = Crud<Usuario>.GetById(id);
            return View(data);
        }

        // POST: UsuariosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, Usuario data)
        {
            try
            {
                Crud<Usuario>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }
        public IActionResult GetUsersByRole(string rol)
        {
            // Obtener todos los usuarios
            var usuarios = Crud<Usuario>.GetAll();

            // Filtrar los usuarios por rol
            var usuariosEnRol = usuarios.Where(u => u.Rol == rol).ToList();

            // Verificar si se encontraron usuarios
            if (usuariosEnRol.Count == 0)
            {
                TempData["Error"] = $"No se encontraron usuarios con el rol '{rol}'.";
            }

            return View(usuariosEnRol);
        }

    }
}
