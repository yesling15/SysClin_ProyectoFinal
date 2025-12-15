using SysClin.Models;
using SysClin.Models.ViewModel;
using SysClin.Utils;
using System.Linq;
using System.Web.Mvc;

namespace SysClin.Controllers
{
    public class LoginController : BaseController
    {      
        private readonly ISysClinContext db;

        public LoginController(ISysClinContext contexto)

        {
            db = contexto;  // Asigna el contexto inyectado
        }

        public LoginController()
        {
            db = new SysClinEntities();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Búsqueda del usuario por correo
            var usuario = db.Usuario
                            .FirstOrDefault(u => u.Correo_Electronico == model.Correo);

            if (usuario == null)
            {
                // Marca para SweetAlert
                ViewBag.LoginError = true;
                return View(model);
            }

            // Verificación de contraseña usando el mismo método de hash que en el registro
            var hashIngresado = PasswordHelper.Hash(model.Contrasena);

            if (usuario.Contrasena != hashIngresado)
            {
                // Marca para SweetAlert
                ViewBag.LoginError = true;
                return View(model);
            }

            // Usuario autenticado: se almacena en sesión            
            if (System.Web.HttpContext.Current != null &&
                System.Web.HttpContext.Current.Session != null)
            {
                System.Web.HttpContext.Current.Session["UsuarioLogueado"] = usuario;
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session["UsuarioLogueado"] = null;
            return RedirectToAction("Index", "Login");
        }
        
    }
}