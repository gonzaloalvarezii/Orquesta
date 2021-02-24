using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Orquesta.Models;

namespace Orquesta.Controllers
{
    public class ClientesController : Controller
    {
        // GET: Clientes
        public ActionResult Index(string busqueda)
        {



            List<Cliente> cli = new List<Cliente>();

            Cliente cli1 = new Cliente();

            cli1.Nombre = "Gonzalo";
            cli1.RUT = "110322520014";


            Cliente cli2 = new Cliente();

            cli2.Nombre = "Diego";
            cli2.RUT = "xxxxxxxxxx";

            cli.Add(cli1);
            cli.Add(cli2);

            if (!String.IsNullOrEmpty(busqueda))
            {
                var aux = cli.Where(x => x.Nombre == busqueda);
                return View(aux);
            }
            else
            {
                return View(cli);

            }


        }

        public ActionResult getClientes(string busqueda)
        {



            List<Cliente> cli = new List<Cliente>();

            Cliente cli1 = new Cliente();

            cli1.Nombre = "Gonzalo";
            cli1.RUT = "110322520014";


            Cliente cli2 = new Cliente();

            cli2.Nombre = "Diego";
            cli2.RUT = "xxxxxxxxxx";

            cli.Add(cli1);
            cli.Add(cli2);


            if (!String.IsNullOrEmpty(busqueda))
            {
                cli.Where(x => x.Nombre == busqueda);

            }

            return View(cli);
        }

        [HttpPost]
      public JsonResult getClientes2(string busqueda)
        {

            List<Cliente> cli = new List<Cliente>();

            Cliente cli1 = new Cliente();

            cli1.Nombre = "Gonzalo";
            cli1.RUT = "110322520014";


            Cliente cli2 = new Cliente();

            cli2.Nombre = "Diego";
            cli2.RUT = "xxxxxxxxxx";

            cli.Add(cli1);
            cli.Add(cli2); 


            if (!String.IsNullOrEmpty(busqueda))
            {
                List<Cliente> aux = cli.Where(x => x.Nombre == busqueda).ToList();
              //   aux = (cli.Where(x => x.Nombre == busqueda).ToList().Count>1)? cli.Where(x => x.Nombre == busqueda).ToList() :null;/*/
                 return Json(aux);

            }
            else
            {
                return Json(cli);

            }
        }

        public ActionResult create() {

            return View();
        }

    }
}