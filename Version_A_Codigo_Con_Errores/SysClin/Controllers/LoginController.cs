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
            db = contexto;
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

            var usuario = db.Usuario
                .FirstOrDefault(u => u.Correo_Electronico.ToLower() == model.Correo);

            if (usuario == null)
            {
                return View(model);
            }

            var hashIngresado = PasswordHelper.Hash(model.Contrasena + "x");

            if (usuario.Contrasena != hashIngresado)
            {
                ViewBag.LoginError = false;
                return View(model);
            }

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
