//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using TuneCastModelo;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace TuneCast.MVC.Controllers
//{
//    public class AccountController : Controller
//    {
//        // Lista temporal de usuarios (en un sistema real esto vendría de tu API)
//        private static List<Usuario> _usuarios = new List<Usuario>
//        {
//            new Usuario { Id = 1, Nombre = "Admin", Email = "admin@example.com", Contraseña = "admin123" }
//        };

//        [HttpGet]
//        public IActionResult Login(string returnUrl = null)
//        {
//            ViewData["ReturnUrl"] = returnUrl;
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Login(string email, string contraseña, bool rememberMe = false, string returnUrl = null)
//        {
//            // Validación simple contra la lista en memoria
//            var usuario = _usuarios.Find(u => u.Email == email && u.Contraseña == contraseña);

//            if (usuario != null)
//            {
//                var claims = new List<Claim>
//                {
//                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
//                    new Claim(ClaimTypes.Name, usuario.Nombre),
//                    new Claim(ClaimTypes.Email, usuario.Email)
//                };

//                var claimsIdentity = new ClaimsIdentity(
//                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

//                await HttpContext.SignInAsync(
//                    CookieAuthenticationDefaults.AuthenticationScheme,
//                    new ClaimsPrincipal(claimsIdentity),
//                    new AuthenticationProperties
//                    {
//                        IsPersistent = rememberMe
//                    });

//                // Redirigir a la página original solicitada (o a la página principal)
//                if (Url.IsLocalUrl(returnUrl) && returnUrl != "/")
//                {
//                    return Redirect(returnUrl);  // Si había una URL de retorno, redirige ahí
//                }

//                return RedirectToAction("Index", "Home");  // Si no, redirige a la página de inicio
//            }

//            ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Logout()
//        {
//            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//            return RedirectToAction("Index", "Home");
//        }
//    }

//}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using TuneCastAPIConsumer;  // Para usar el Crud<T>
using TuneCastModelo;  // Para usar el modelo Usuario

namespace TuneCast.MVC.Controllers
{
    public class AccountController : Controller
    {
        // Acción para mostrar el formulario de Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Acción para manejar el login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string contraseña, bool rememberMe = false, string returnUrl = null)
        {
            // Llamada al API para obtener todos los usuarios (puedes optimizar esto buscando por email)
            var usuarios = Crud<Usuario>.GetAll();  // Llamada a la API para obtener todos los usuarios registrados

            // Buscar el usuario con el email y la contraseña proporcionados
            var usuario = usuarios.FirstOrDefault(u => u.Email == email && u.Contraseña == contraseña);

            if (usuario != null)
            {
                // Crear los claims para la autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Iniciar sesión con las cookies
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = rememberMe
                    });

                // Si había una URL de retorno (la página que el usuario intentó acceder), redirige allí
                if (Url.IsLocalUrl(returnUrl) && returnUrl != "/")
                {
                    return Redirect(returnUrl);  // Si había una URL de retorno, redirige ahí
                }

                // Si no, redirige al Home
                return RedirectToAction("Index", "Home");
            }

            // Si no se encontró el usuario, muestra un error
            ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            return View();
        }

        // Acción para mostrar el formulario de registro
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Acción para registrar un nuevo usuario
        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string email, string contraseña)
        {
            // Verificar si el email ya está registrado (llamamos a la API)
            var usuarios = Crud<Usuario>.GetAll();  // Llamada a la API para obtener todos los usuarios registrados
            var usuarioExistente = usuarios.FirstOrDefault(u => u.Email == email);

            if (usuarioExistente != null)
            {
                ModelState.AddModelError(string.Empty, "El correo electrónico ya está registrado.");
                return View();
            }

            // Crear un nuevo usuario
            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Email = email,
                Contraseña = contraseña // En un sistema real, deberías cifrar la contraseña
            };

            // Llamar a la API para crear el usuario
            try
            {
                var usuarioCreado = await Crud<Usuario>.Create(nuevoUsuario);  // Llamada al método Create del Crud
                if (usuarioCreado != null)
                {
                    return RedirectToAction("Login");  // Redirigir al login después de un registro exitoso
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al registrar el usuario.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al registrar el usuario: {ex.Message}");
                return View();
            }
        }

        // Acción para manejar el Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    




    }

}

