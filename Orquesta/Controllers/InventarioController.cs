using ExcelDataReader;
using Orquesta.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Orquesta.Controllers
{
    public class InventarioController : Controller
    {
        // GET: Inventario
        public ActionResult Index()
        {
            List<InventarioDTO> datos = new List<InventarioDTO>();

            try
            {
                datos = lista_inventarios();
            }
            catch (Exception ex)
            {
                ex.ToString();

            }

            return View(datos);

        }

        public List<InventarioDTO> lista_inventarios()
        {
            dev_orquestaEntities db = new dev_orquestaEntities();

            List<InventarioDTO> datos = new List<InventarioDTO>();

            try
            {
                datos = (from inv in db.INVENTARIO
                         select new InventarioDTO { Fac_numero = inv.Fac_numero, Fec_factura = inv.Fec_Factura, Fec_ingreso = inv.Fec_ingreso, Proveedor = inv.Proveedor, Propietario = inv.Propietario, Id_Inventario = inv.Id_Inventario }).ToList();
            }
            catch (Exception ex)
            {
                ex.ToString();

            }

            return datos;

        }

        public ActionResult Nuevo_SIM()
        {
            return View();
        }

        public ActionResult Nuevo_POS()
        {
            return View();
        }

        public ActionResult create()
        {

            return View();

        }
        public ActionResult create2(HttpPostedFileBase file, string Fac_numero, DateTime Fec_ingreso, string Proveedor, string cboUbicacion, DateTime fec_factura, string Propietario)
        {


            string ruta = ConfigurationSettings.AppSettings["ruta_archivos"].ToString();
            StringBuilder sql = new StringBuilder();
            string columnas = string.Empty;
            string filePath = string.Empty;
            string _FileName = string.Empty;
            int last_insert_id = 0;
            List<string> repetidos = new List<string>();
            int contSIM = 0;
            int contPOS = 0;
            INVENTARIO inv = null;
            bool modificar = true;

            try
            {
                if (file == null)
                {
                    return View("create");
                }

                if (file.ContentLength > 0)
                {
                    _FileName = Path.GetFileName(file.FileName);
                    filePath = ruta + _FileName;
                    file.SaveAs(filePath);

                }
            }
            catch (IOException)
            {
                ViewBag.Error = "Error leyendo o guardando archivo. Verifique que el mismo exista y no esté en uso";
                return View("create");
            }

            using (var stream = file.InputStream)
            // using (var stream = System.IO.File.Open(_FileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {

                    using (var db = new dev_orquestaEntities())
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            // var result = reader.AsDataSet();
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                // Gets or sets a callback to obtain configuration options for a DataTable. 
                                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true,

                                }
                            });


                            DataTable table = result.Tables[0];

                            //var dtResultado = table.Rows.Cast<DataRow>().Where(row => !Array.TrueForAll(row.ItemArray, value => { return row.ItemArray[0].ToString().Length == 0; }));


                            if (!validarDT(new string[] { "Serie", "Modelo", "Conectividad", "ID_Modelo", "ID_Conectividad", "Modelo_aux" }, table))
                            {
                                ViewBag.Error = "Excel inválido";
                                return View("create");
                            }


                            DataTable table_SIM = result.Tables[1];


                            if (!validarDT(new string[] { "Serie", "Operadora", "PIN", "PUK", "Phone Number", "ID_Operadora", "Operadora_aux" }, table_SIM))
                            {
                                ViewBag.Error = "Excel inválido";
                                return View("create");
                            }

                            table.Rows.RemoveAt(0);
                            table_SIM.Rows.RemoveAt(0);

                            //si factura existe la actualizo con sus items
                            if (Fac_numero != "")
                            {
                                inv = db.INVENTARIO.FirstOrDefault(ct => ct.Fac_numero.Equals(Fac_numero));
                            }


                            if (inv == null)
                            {
                                inv = new INVENTARIO();
                                inv.Proveedor = Proveedor;
                                inv.Fec_ingreso = Fec_ingreso;
                                inv.Fac_numero = Fac_numero;
                                inv.Propietario = Propietario;
                                inv.Fec_Factura = fec_factura;
                                modificar = false;
                            }


                            foreach (DataRow dr in table.Rows)
                            {
                                POS p = new POS();
                                string serie_POS = validarDataRow(dr, 0, 0);

                                if (serie_POS == "")
                                {
                                    continue;
                                }

                                if (serie_POS.Length != 24)
                                {
                                    repetidos.Add("Formato incorrecto POS - " + serie_POS);
                                }
                                else if (validar_articulo_duplicado(db, serie_POS, 1))
                                {
                                    repetidos.Add("Repetido POS - " + serie_POS);
                                }
                                else
                                {

                                    p.Id_Estado = 1;
                                    p.Id_Categoria = 1;
                                    p.Nro_serie = serie_POS;
                                    p.Descripcion = "";
                                    p.Id_Modelo = Convert.ToInt32(validarDataRow(dr, 3, 1));

                                    db.ARTICULO.Add(p);
                                    db.SaveChanges();

                                    last_insert_id = p.Id_Articulo;

                                    DETALLE_INVENTARIO det_inv = new DETALLE_INVENTARIO();
                                    det_inv.Id_Articulo = last_insert_id;
                                    det_inv.Id_Ubicacion = Convert.ToInt32(cboUbicacion);
                                    contPOS = contPOS + 1;


                                    inv.DETALLE_INVENTARIO.Add(det_inv);
                                }
                            }



                            foreach (DataRow dr2 in table_SIM.Rows)
                            {
                                SIM s = new SIM();
                                string serie_SIM = validarDataRow(dr2, 0, 0);

                                if (serie_SIM == "")
                                {
                                    continue;
                                }

                                if (validar_articulo_duplicado(db, serie_SIM, 2))
                                {
                                    repetidos.Add("Repetido SIM  -" + serie_SIM);
                                }
                                else
                                {
                                    s.Id_Estado = 1;
                                    s.Id_Categoria = 2;
                                    s.Nro_serie = serie_SIM;
                                    s.Descripcion = "";
                                    s.Id_Tipo_Operador = Convert.ToInt32(validarDataRow(dr2, 5, 1)); ;
                                    db.ARTICULO.Add(s);

                                    db.SaveChanges();
                                    last_insert_id = s.Id_Articulo;

                                    DETALLE_INVENTARIO det_inv_sim = new DETALLE_INVENTARIO();
                                    det_inv_sim.Id_Articulo = last_insert_id;
                                    det_inv_sim.Id_Ubicacion = Convert.ToInt32(cboUbicacion); ;

                                    inv.DETALLE_INVENTARIO.Add(det_inv_sim);
                                    contSIM = contSIM + 1;
                                }

                            }

                            //hay nuevos
                            if (contSIM > 0 || contPOS > 0)
                            {
                                if (modificar)
                                {
                                    db.INVENTARIO.Attach(inv);
                                }
                                else
                                {
                                    db.INVENTARIO.Add(inv);
                                }

                                db.SaveChanges();
                                dbContextTransaction.Commit();

                            }
                        }

                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }

                            dbContextTransaction.Rollback();
                            throw;
                        }
                    }


                }

                if (repetidos.Count > 0)
                {
                    ViewBag.lista_repetidos = repetidos;
                    ViewBag.POSSIM = "Guardados: POS-" + contPOS + ", SIM-" + contSIM;
                    return View("create");
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }


        }

        public bool validarDT(string[] nombreColumnas, DataTable dt)
        {

            bool retorno = true;

            if (nombreColumnas.Length != dt.Columns.Count)
            {
                return false;
            }

            for (int i = 0; i <= dt.Columns.Count - 1; i++)
            {

                if (nombreColumnas[i].ToString().Trim() != dt.Columns[i].ColumnName.Trim())
                {
                    return false;
                }

            }

            return retorno;

        }

        public string validarDataRow(DataRow dr, int pos, int tipo_dato)
        {
            string retorno = string.Empty;

            if (dr != null)
            {
                if (dr[pos] != null)
                {

                    if (dr[pos].ToString().Trim() != "")
                    {
                        retorno = dr[pos].ToString().Trim();
                    }
                }

            }

            // tipo de dato numerico = 1
            if (tipo_dato == 1 && retorno == "")
            {
                retorno = "0";
            }

            return retorno;

        }

        public bool validar_articulo_duplicado(dev_orquestaEntities db, string nro_serie, int tipo_articulo)
        {
            bool existe = false;
            ARTICULO art = new ARTICULO();

            if (tipo_articulo == 1)
            {
                art = db.ARTICULO.OfType<POS>().FirstOrDefault(ct => ct.Nro_serie.Equals(nro_serie));
            }
            else
            {
                art = db.ARTICULO.OfType<SIM>().FirstOrDefault(ct => ct.Nro_serie.Equals(nro_serie));
            }

            if (art != null)
            {
                existe = true;
            }

            return existe;
        }

        public ActionResult Details(int id)
        {
            List<Detalle_InventarioDTO> datos = new List<Detalle_InventarioDTO>();
            datos = lista_datos_detalles(id);

            return View(datos);
        }

        public List<Detalle_InventarioDTO> lista_datos_detalles(int id)
        {

            List<Detalle_InventarioDTO> datos = new List<Detalle_InventarioDTO>();
            dev_orquestaEntities db = new dev_orquestaEntities();

            datos = (from det in db.DETALLE_INVENTARIO
                     join ar in db.ARTICULO on det.Id_Articulo equals ar.Id_Articulo
                     join es in db.ESTADO on ar.Id_Estado equals es.Id_Estado
                     join ca in db.CATEGORIA on ar.Id_Categoria equals ca.Id_Categoria
                     join ub in db.UBICACION on det.Id_Ubicacion equals ub.Id_Ubicacion
                     where det.Id_Inventario == id
                     select new Detalle_InventarioDTO { Id_Detalle_Inventario = det.Id_Detalle_Inventario, Id_articulo = det.Id_Articulo, Categoria = ca.Descripcion, Ubicacion = ub.Descripcion, Estado = es.Descripcion }).ToList();


            return datos;

        }

        public ActionResult Delete_Detalles(int id_det)
        {
            dev_orquestaEntities db = new dev_orquestaEntities();
            DETALLE_INVENTARIO detalle = new DETALLE_INVENTARIO();
            List<Detalle_InventarioDTO> datos = new List<Detalle_InventarioDTO>();
            ARTICULO art = new ARTICULO() { };

            detalle = db.DETALLE_INVENTARIO
                          .Include("ARTICULO").FirstOrDefault(ct => ct.Id_Detalle_Inventario.Equals(id_det));


            if (detalle.ARTICULO.Id_Categoria == 1)
            {
                POS po = db.ARTICULO.OfType<POS>().FirstOrDefault(ct => ct.Id_Articulo.Equals(detalle.ARTICULO.Id_Articulo));
                db.ARTICULO.Remove(po);
            }
            else
            {
                SIM si = db.ARTICULO.OfType<SIM>().FirstOrDefault(ct => ct.Id_Articulo.Equals(detalle.ARTICULO.Id_Articulo));
                db.ARTICULO.Remove(si);
            }

            db.DETALLE_INVENTARIO.Remove(detalle);
            db.SaveChanges();

            datos = lista_datos_detalles(detalle.Id_Inventario);
            return View("Details", datos);
        }

        public ActionResult Delete_Inventario(int id_inv)
        {
            dev_orquestaEntities db = new dev_orquestaEntities();
            INVENTARIO detalle = new INVENTARIO();
            List<InventarioDTO> datos = new List<InventarioDTO>();
            ARTICULO art = new ARTICULO() { };

            detalle = db.INVENTARIO
                          .Include("DETALLE_INVENTARIO").Include("DETALLE_INVENTARIO.ARTICULO").FirstOrDefault(ct => ct.Id_Inventario.Equals(id_inv));


            foreach (DETALLE_INVENTARIO item in detalle.DETALLE_INVENTARIO)
            {

                if (item.ARTICULO.Id_Categoria == 1)
                {
                    POS po = db.ARTICULO.OfType<POS>().FirstOrDefault(ct => ct.Id_Articulo.Equals(item.ARTICULO.Id_Articulo));
                    db.ARTICULO.Remove(po);
                }
                else
                {
                    SIM si = db.ARTICULO.OfType<SIM>().FirstOrDefault(ct => ct.Id_Articulo.Equals(item.ARTICULO.Id_Articulo));
                    db.ARTICULO.Remove(si);
                }

            }

            db.DETALLE_INVENTARIO.RemoveRange(detalle.DETALLE_INVENTARIO);
            db.SaveChanges();

            db.INVENTARIO.Remove(detalle);
            db.SaveChanges();

            datos = lista_inventarios();
            return View("Index", datos);
        }


        public ActionResult Editar_Detalle_Inventario(int id)
        {
            dev_orquestaEntities db = new dev_orquestaEntities();
            Detalle_InventarioDTO detalle = new Detalle_InventarioDTO();

            //detalle = db.DETALLE_INVENTARIO
            //                   .Include("ARTICULO").Include("UBICACION").Include("ARTICULO.ESTADO").FirstOrDefault(ct => ct.Id_Detalle_Inventario.Equals(id));



            detalle = (from det in db.DETALLE_INVENTARIO
                     join ar in db.ARTICULO on det.Id_Articulo equals ar.Id_Articulo
                     join es in db.ESTADO on ar.Id_Estado equals es.Id_Estado
                     join ub in db.UBICACION on det.Id_Ubicacion equals ub.Id_Ubicacion
                     where det.Id_Detalle_Inventario == id
                     select new Detalle_InventarioDTO {Ubicacion = ub.Descripcion, Estado = es.Descripcion }).First();


            return View(detalle);
        }

        public ActionResult Guardar_Detalle_Inventario(int id, string Ubicacion, string Estado)
        {
            dev_orquestaEntities db = new dev_orquestaEntities();
            DETALLE_INVENTARIO detalle = new DETALLE_INVENTARIO();

            ARTICULO art = new ARTICULO() { };

            detalle = db.DETALLE_INVENTARIO
                          .Include("ARTICULO").FirstOrDefault(ct => ct.Id_Detalle_Inventario.Equals(id));

            detalle.Id_Ubicacion = 1;
            detalle.ARTICULO.ESTADO.Id_Estado = 1;

            db.DETALLE_INVENTARIO.Attach(detalle);
            db.SaveChanges();

            return View();
        }
    }
}