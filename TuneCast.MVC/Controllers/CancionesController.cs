﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TuneCastAPIConsumer;
using TuneCastModelo;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TuneCast.MVC.Controllers
{
    public class CancionesController : Controller
    {
        // GET: CancionesController
        public ActionResult Index()
        {
            var data = Crud<Cancion>.GetAll();
            return View(data);
        }

        // GET: CancionesController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Cancion>.GetById(id);
            return View(data);
        }

        // GET: CancionesController/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: CancionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Cancion data, IFormFile archivo)
        {
            try
            {
                if (archivo != null && archivo.Length > 0)
                {
                    // Validar que el archivo es de tipo audio (mp3, wav, ogg)
                    var extensionesPermitidas = new[] { ".mp3", ".wav", ".ogg" };
                    var extension = Path.GetExtension(archivo.FileName).ToLower();

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError("", "Solo se permiten archivos MP3, WAV o OGG.");
                        return View();
                    }

                    // Guardar el archivo en el directorio del servidor
                    var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "canciones", archivo.FileName);
                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await archivo.CopyToAsync(stream);
                    }

                    // Asignar la ruta del archivo a la canción
                    data.RutaArchivo = "/canciones/" + archivo.FileName;

                    // Crear la canción usando el API (Crud<T>)
                    await Crud<Cancion>.Create(data);
                    return RedirectToAction(nameof(Index));  // Redirigir al índice de canciones
                }

                ModelState.AddModelError("", "El archivo no es válido.");
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }
    

        //// POST: CancionesController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(Cancion data)
        //{
        //    try
        //    {
        //       Crud<Cancion>.Create(data);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", ex.Message);
        //        return View(data);
        //    }
        //}

        // GET: CancionesController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Cancion>.GetById(id);
            return View(data);
        }

        // POST: CancionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Cancion data)
        {
            try
            {
                Crud<Cancion>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: CancionesController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Cancion>.GetById(id);
            return View(data);
        }

        // POST: CancionesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Cancion data)
        {
            try
            {
                Crud<Cancion>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }
    }
}
