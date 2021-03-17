using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Orquesta.Models;

namespace Orquesta.Controllers
{
    public class PedidoController : Controller
    {
        // GET: Pedido
        public ActionResult Index()
        {
            return View();
        }   
        public ActionResult getPedidos()
        {
            //soy un cambio
         // if (!String.IsNullOrEmpty(pedido)) {
            dev_orquestaEntities db = new dev_orquestaEntities();

            List<getPedidos> datos = new List<getPedidos>();

            try
            {
             datos = (from ped in db.PEDIDO
                                          join cans in db.CANAL on ped.Id_Canal equals cans.Id_Canal
                                          join usu in db.USUARIO on ped.Id_Usuario equals usu.Id_Usuario
                                          select new getPedidos {Id_Pedido=ped.Id_Pedido, canal = cans.Descripcion, usuario = usu.Nombre, Fecha_Ingreso = ped.Fecha_Ingreso, Comentario = ped.Comentario }).ToList();
            }
            catch (Exception ex) {
                ex.ToString();
               
            }

            return View(datos);

            
        }

        public ActionResult create()
        {
            cargarCombos();
            return View();

        }

        public void cargarCombos() {

            dev_orquestaEntities db = new dev_orquestaEntities();

            var usu = db.USUARIO.ToList();
            ViewBag.Usuarios = new SelectList(usu, "Id_Usuario", "Nombre");

            var can = db.CANAL.ToList();
            ViewBag.Canales = new SelectList(can, "Id_Canal", "Descripcion");

            var cmbPlan = db.PLANES.ToList();
            ViewBag.Planes = new SelectList(cmbPlan, "Id_Plan", "Descripcion");


            List<getPlanCount> lstPlan = (from a in db.ARTICULO
                                          join inv in db.DETALLE_INVENTARIO on a.Id_Articulo equals inv.Id_Articulo
                                          join po in  db.ARTICULO.OfType<POS>() on a.Id_Articulo equals po.Id_Articulo
                                          join mod in db.MODELO on po.Id_Modelo equals mod.Id_Modelo
                                          join modPl in db.MODELO_PLAN on mod.Id_Modelo equals modPl.Id_Modelo
                                          join Pl in db.PLANES on modPl.Id_Plan equals Pl.Id_Plan
                                          group new { Pl, mod} by new { Pl.Descripcion,mod.Nombre } into grp
                                          select new getPlanCount { Descripcion = grp.Key.Descripcion,Modelo=grp.Key.Nombre, Cantidad = grp.Count() }).ToList();
            ViewBag.tblPlan = lstPlan;


            List<Tipo_OperadorDTO> lstOperador = (from s in db.ARTICULO.OfType<SIM>()
                                          join to in db.TIPO_OPERADOR on s.Id_Tipo_Operador equals to.Id_Tipo_Operador
                                          group new { to } by new { to.Id_Tipo_Operador, to.Descripcion } into grpTO
                                          select new Tipo_OperadorDTO {Id_Tipo_Operador= grpTO.Key.Id_Tipo_Operador, Descripcion = grpTO.Key.Descripcion}).ToList();
            ViewBag.Operador = lstOperador;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult guardar(PEDIDO ped, string detalles)
        {
            ViewBag.result = "";

            if (!ModelState.IsValid)
            {
                return View();
            }

            using (var db = new dev_orquestaEntities())
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    ped.Id_Pedido = 0;
                    ped.Fecha_Ingreso = DateTime.Now;
                    ped.USUARIO  = (USUARIO)Session["User"];
                    db.PEDIDO.Add(ped);
                    db.SaveChanges();

                    int last_insert_id = ped.Id_Pedido;

                    if (detalles != "" && detalles != null)
                    {
                        char caracter = '}';
                        char caracter2 = '-';

                        string[] details = detalles.Split(caracter);

                        DETALLE_PEDIDO det_ped = new DETALLE_PEDIDO();

                        for (int i = 0; i < details.Length; i++)
                        {

                            string[] separo = details[i].Split(caracter2);

                            det_ped.Id_Pedido = last_insert_id;
                            det_ped.Cantidad = Convert.ToInt32(separo[2].Trim());

                            db.DETALLE_PEDIDO.Add(det_ped);
                            db.SaveChanges();
                           
                        }

                        dbContextTransaction.Commit();

                        return RedirectToAction("getPedidos");

                    }
                    else 
                    {
                        //sin detalle no hay pedido
                        dbContextTransaction.Rollback();
                        ViewBag.result = "Ingrese Detalle";

                    }
                }
                catch {

                    dbContextTransaction.Rollback();
                    ViewBag.result = "Error Cargando Datos";
                }
            }

            cargarCombos();
            return View("create");
          
        }

        public ActionResult Delete()
        {
            cargarCombos();
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (var db = new dev_orquestaEntities())
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                  List<DETALLE_PEDIDO> det = (from deta in db.DETALLE_PEDIDO 
                                                  select new DETALLE_PEDIDO { Id_Pedido = id }).ToList();

                   PEDIDO ped = db.PEDIDO.Find(id);

                    foreach (DETALLE_PEDIDO items in det) {
                        db.DETALLE_PEDIDO.Remove(items);
                    }

                    db.PEDIDO.Remove(ped);
                    db.SaveChanges();
                }
                catch (DataException/* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    return RedirectToAction("Delete", new { id = id, saveChangesError = true });
                }
            }
            return RedirectToAction("Create");

        }
        
        [HttpPost]
        public JsonResult getStockPlanes(int busqueda)
        {

            List<getPlanCount> plan = new List<getPlanCount>();

            if (!String.IsNullOrEmpty(busqueda.ToString()))
            {
                using (var db = new dev_orquestaEntities()) {

                    List<getPlanCount> lstPlan =( from po in db.ARTICULO.OfType<POS>()
                                                  join inv in db.DETALLE_INVENTARIO on po.Id_Articulo equals inv.Id_Articulo
                                                  join mod in db.MODELO on po.Id_Modelo equals mod.Id_Modelo
                                                  where mod.Id_Modelo== (busqueda.ToString()=="" ? mod.Id_Modelo : busqueda)
                             group po by mod.Nombre into grp
                             select new getPlanCount { Descripcion = grp.Key, Cantidad = grp.Count()}).ToList();

                    return Json(lstPlan);
            }

            }
            else
            {
                return Json(plan);

            }
        }




        public ActionResult getConfiguracionPOS()
        {
            dev_orquestaEntities db = new dev_orquestaEntities();


            List<ModeloDTO> ModeloDTO = (from mode in db.MODELO

                                         select new ModeloDTO { Id_Modelo = mode.Id_Modelo, Nombre = mode.Nombre }).ToList();

            ViewBag.Modelo = ModeloDTO;

          

            return View();
        }


        [HttpPost]
        public JsonResult InsertValue(getConfiguracionPOS[] itemlist)
        {
            dev_orquestaEntities db = new dev_orquestaEntities();

            List<ARTICULO> la = new List<ARTICULO>();
            List<POS_SIM> p_s = new List<POS_SIM>();
            List<POS> lp = new List<POS>();
            List<SIM> ls = new List<SIM>();

           

            foreach (getConfiguracionPOS i in itemlist)
            {
                
                POS p = new POS();
                SIM s = new SIM();
                POS_SIM ps = new POS_SIM();
            
                p.Id_Categoria = 1;
                p.Id_Estado = 1;
                p.Descripcion = i.Descripcion;
                p.Stock = "2";
                p.Pk_version = i.Package;
                p.Id_Modelo = Convert.ToInt32(i.Modelo);
                p.SW_version = i.Software;
                p.Nro_serie = i.Nro_serie_POS;


                s.Id_Categoria = 2;
                s.Id_Estado = 1;
                s.Descripcion = i.Descripcion;
                s.Stock = "2";
                s.Puk = "0";
                s.Phone_Numer = "094800578";
                s.PIN = "6464";
                s.Nro_serie = i.Nro_serie_SIM;

                db.ARTICULO.Add(p);
                db.SaveChanges();

                db.ARTICULO.Add(s);
                db.SaveChanges();

                int last_insert_id_a1 = p.Id_Articulo;
                int last_insert_id_a2 = s.Id_Articulo;

                
                ps.Id_POS = last_insert_id_a1;
                ps.Id_SIM = last_insert_id_a2;
               
                db.POS_SIM.Add(ps);
                db.SaveChanges();



            }
            return Json("Ok");
        }

    }
}