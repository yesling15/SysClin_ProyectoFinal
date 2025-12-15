using SysClin.Models;
using SysClin.Models.ViewModel;
using SysClin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SysClin.Controllers
{
    public class UsuarioController : BaseController
    {       
        private readonly ISysClinContext db;

        public UsuarioController()
        {
            db = new SysClinEntities();
        }

        public UsuarioController(ISysClinContext context)
        {
            db = context;
        }

        // GET: Usuario/Registro
        [HttpGet]
        public ActionResult Registro()
        {
            CargarCombosRegistro();
            return View(new UsuarioViewModel());
        }

        // POST: Usuario/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(UsuarioViewModel model)
        {
            // Recargar listas desplegables en caso de error de validación
            CargarCombosRegistro();

            // Validaciones adicionales no cubiertas por DataAnnotations
            ValidarNumeroIdentificacionPorTipo(model);
            ValidarFechaNacimiento(model);

            if (!ModelState.IsValid)
                return View(model);

            // Verificación de unicidad de correo
            if (db.Usuario.Any(u => u.Correo_Electronico == model.Correo_Electronico))
            {
                ModelState.AddModelError("Correo_Electronico",
                    "El correo electrónico ya se encuentra registrado.");
            }

            // Verificación de unicidad de identificación
            if (db.Usuario.Any(u => u.Numero_Identificacion == model.Numero_Identificacion))
            {
                ModelState.AddModelError("Numero_Identificacion",
                    "El número de identificación ya se encuentra registrado.");
            }

            if (!ModelState.IsValid)
                return View(model);

            // Creación de la entidad Usuario
            var usuario = new Usuario
            {
                ID_Tipo_Usuario = model.ID_Tipo_Usuario.Value,
                ID_Tipo_Identificacion = model.ID_Tipo_Identificacion.Value,
                Numero_Identificacion = model.Numero_Identificacion,
                Nombre = model.Nombre,
                Primer_Apellido = model.Primer_Apellido,
                Segundo_Apellido = model.Segundo_Apellido,
                Correo_Electronico = model.Correo_Electronico,

                // Encriptación de contraseña
                Contrasena = PasswordHelper.Hash(model.Contrasena),

                Telefono = model.Telefono,
                Fecha_Nacimiento = model.Fecha_Nacimiento.Value
            };

            db.Usuario.Add(usuario);
            db.SaveChanges();

            // Validación de tipo profesional (redirige a completar perfil)
            int idProfesional = db.TipoUsuario
                                  .Where(t => t.Nombre == "Profesional")
                                  .Select(t => t.ID_Tipo_Usuario)
                                  .FirstOrDefault();

            if (usuario.ID_Tipo_Usuario == idProfesional)
            {
                return RedirectToAction("Crear", "PerfilProfesional",
                    new { idUsuario = usuario.ID_Usuario });
            }

            // MARCA para que la vista muestre el SweetAlert
            ViewBag.RegistroExitoso = true;

            // Volver a la misma vista para que aparezca el SweetAlert
            return View(model);
        }

        // GET: Usuario/EditarPerfil
        [HttpGet]
        public ActionResult EditarPerfil()
        {
            // El usuario logueado siempre está en sesión
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            var usuario = db.Usuario
                            .Where(u => u.ID_Usuario == usuarioSesion.ID_Usuario)
                            .FirstOrDefault();

            if (usuario == null)
                return HttpNotFound();

            var model = new UsuarioPerfilViewModel
            {
                ID_Usuario = usuario.ID_Usuario,
                TipoUsuarioNombre = usuario.TipoUsuario.Nombre,
                TipoIdentificacionNombre = usuario.TipoIdentificacion.Nombre,
                Numero_Identificacion = usuario.Numero_Identificacion,

                Nombre = usuario.Nombre,
                Primer_Apellido = usuario.Primer_Apellido,
                Segundo_Apellido = usuario.Segundo_Apellido,
                Correo_Electronico = usuario.Correo_Electronico,
                Telefono = usuario.Telefono,
                Fecha_Nacimiento = usuario.Fecha_Nacimiento
            };

            return View(model);
        }

        // POST: Usuario/EditarPerfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPerfil(UsuarioPerfilViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Validación manual: fecha en el pasado
            if (model.Fecha_Nacimiento >= DateTime.Today)
            {
                ModelState.AddModelError("Fecha_Nacimiento",
                    "La fecha de nacimiento debe ser una fecha en el pasado.");
                return View(model);
            }

            // Verificar correo duplicado
            if (db.Usuario.Any(u => u.Correo_Electronico == model.Correo_Electronico &&
                                    u.ID_Usuario != model.ID_Usuario))
            {
                ModelState.AddModelError("Correo_Electronico",
                    "El correo electrónico ya se encuentra registrado por otro usuario.");
                return View(model);
            }

            var usuario = db.Usuario.Find(model.ID_Usuario);
            if (usuario == null)
                return HttpNotFound();

            // Actualizar SOLO los campos editables
            usuario.Nombre = model.Nombre;
            usuario.Primer_Apellido = model.Primer_Apellido;
            usuario.Segundo_Apellido = model.Segundo_Apellido;
            usuario.Correo_Electronico = model.Correo_Electronico;
            usuario.Telefono = model.Telefono;
            usuario.Fecha_Nacimiento = model.Fecha_Nacimiento;

            db.SaveChanges();

            // Para SweetAlert
            ViewBag.EditarPerfilExitoso = true;

            // Recargar nuevamente el modelo para que no se borren los datos
            var usuarioActualizado = db.Usuario.Find(model.ID_Usuario);

            var modeloRefrescado = new UsuarioPerfilViewModel
            {
                ID_Usuario = usuarioActualizado.ID_Usuario,
                TipoUsuarioNombre = usuarioActualizado.TipoUsuario.Nombre,
                TipoIdentificacionNombre = usuarioActualizado.TipoIdentificacion.Nombre,
                Numero_Identificacion = usuarioActualizado.Numero_Identificacion,
                Nombre = usuarioActualizado.Nombre,
                Primer_Apellido = usuarioActualizado.Primer_Apellido,
                Segundo_Apellido = usuarioActualizado.Segundo_Apellido,
                Correo_Electronico = usuarioActualizado.Correo_Electronico,
                Telefono = usuarioActualizado.Telefono,
                Fecha_Nacimiento = usuarioActualizado.Fecha_Nacimiento
            };

            return View(modeloRefrescado);
        }

        [HttpGet]
        public ActionResult CambiarContrasena()
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            var model = new CambiarContrasenaViewModel
            {
                ID_Usuario = usuarioSesion.ID_Usuario
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarContrasena(CambiarContrasenaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = db.Usuario.Find(model.ID_Usuario);
            if (usuario == null)
                return HttpNotFound();

            // 1. Validar contraseña actual
            bool contrasenaActualCorrecta =
                PasswordHelper.Verify(model.ContrasenaActual, usuario.Contrasena);

            if (!contrasenaActualCorrecta)
            {
                ModelState.AddModelError("ContrasenaActual",
                    "La contraseña actual es incorrecta.");
                return View(model);
            }

            // 2. Validar que la nueva NO sea igual a la anterior
            bool mismaContrasena =
                PasswordHelper.Verify(model.NuevaContrasena, usuario.Contrasena);

            if (mismaContrasena)
            {
                ModelState.AddModelError("NuevaContrasena",
                    "La nueva contraseña no puede ser igual a la anterior.");
                return View(model);
            }

            // 3. Guardar nueva contraseña encriptada
            usuario.Contrasena = PasswordHelper.Hash(model.NuevaContrasena);
            db.SaveChanges();

            // Mostrar SweetAlert
            ViewBag.ContrasenaActualizada = true;

            // Devolver el mismo modelo para que funcione correctamente
            return View(model);
        }


        /* ---------------- Métodos de apoyo ---------------- */

        private void CargarCombosRegistro()
        {
            ViewBag.TiposIdentificacion = new SelectList(
                db.TipoIdentificacion.ToList(),
                "ID_Tipo_Identificacion",
                "Nombre");

            ViewBag.TiposUsuario = new SelectList(
                db.TipoUsuario.ToList(),
                "ID_Tipo_Usuario",
                "Nombre");
        }

        private void ValidarFechaNacimiento(UsuarioViewModel model)
        {
            if (model.Fecha_Nacimiento >= DateTime.Today)
            {
                ModelState.AddModelError("Fecha_Nacimiento",
                    "La fecha de nacimiento debe ser una fecha en el pasado.");
            }
        }

        private void ValidarNumeroIdentificacionPorTipo(UsuarioViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Numero_Identificacion))
                return;

            string valor = model.Numero_Identificacion.Trim();

            const int TipoCedula = 1;
            const int TipoDimex = 2;
            const int TipoPasaporte = 3;

            if (model.ID_Tipo_Identificacion == TipoCedula &&
                !Regex.IsMatch(valor, @"^\d{9}$"))
            {
                ModelState.AddModelError("Numero_Identificacion",
                    "La cédula nacional debe contener 9 dígitos numéricos.");
            }
            else if (model.ID_Tipo_Identificacion == TipoDimex &&
                     !Regex.IsMatch(valor, @"^\d{11,12}$"))
            {
                ModelState.AddModelError("Numero_Identificacion",
                    "El número DIMEX debe contener de 11 a 12 dígitos numéricos.");
            }
            else if (model.ID_Tipo_Identificacion == TipoPasaporte &&
                     !Regex.IsMatch(valor, @"^[A-Za-z0-9]{6,20}$"))
            {
                ModelState.AddModelError("Numero_Identificacion",
                    "El pasaporte debe tener entre 6 y 20 caracteres alfanuméricos.");
            }
        }       
    }
}
