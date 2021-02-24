using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orquesta.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(string User, string Pass)
        {
            try
            {
                using (Models.dev_orquestaEntities db = new Models.dev_orquestaEntities())
                {
                    var oUser = (from d in db.USUARIO
                                 where d.Mail == User.Trim() && d.Password == Pass.Trim()
                                 select d).FirstOrDefault();
                    if (oUser == null)
                    {
                        ViewBag.Error = "Usuario o contraseña invalida";
                        return View();
                    }

                    Session["User"] = oUser;
                    Session["UserNombre"] = oUser.Nombre;

                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }

        }
    }
}