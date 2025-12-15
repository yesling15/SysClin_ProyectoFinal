using SysClin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SysClin.Controllers
{
    public class BaseController : Controller
    {
        public BaseController() 
        {
            // Evita errores cuando HttpContext no existe (por ejemplo en pruebas unitarias)
            if (System.Web.HttpContext.Current == null ||
                System.Web.HttpContext.Current.Session == null)
            {
                return;
            }

            var usuarioTO = (Usuario)System.Web.HttpContext.Current.Session["UsuarioLogueado"];
            if (usuarioTO != null)
            {
                ViewBag.UserSession = $"{usuarioTO.Nombre} {usuarioTO.Primer_Apellido}";

            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (System.Web.HttpContext.Current == null ||
                System.Web.HttpContext.Current.Session == null)
            {
                base.OnActionExecuted(filterContext);
                return;
            }

            var usuario = Session["UsuarioLogueado"] as Usuario;

            if (usuario == null)
            {
                // Las vistas públicas usan el layout de login
                ViewBag.Layout = "~/Views/Shared/_LayoutLogin.cshtml";
            }
            else
            {
                switch (usuario.ID_Tipo_Usuario)
                {
                    case 1: // Paciente
                        ViewBag.Layout = "~/Views/Shared/_LayoutPaciente.cshtml";
                        break;

                    case 2: // Profesional
                        ViewBag.Layout = "~/Views/Shared/_LayoutProfesional.cshtml";
                        break;

                    default:
                        ViewBag.Layout = "~/Views/Shared/_LayoutLogin.cshtml";
                        break;
                }
            }

            base.OnActionExecuted(filterContext);
        }

    }
}