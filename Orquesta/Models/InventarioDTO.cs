using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orquesta.Models
{
    public class InventarioDTO
    {        
        public int Id_Inventario { get; set; }

        [Display(Name = "Nro. Factura")]
        
        public string Fac_numero { get; set; }

        [Display(Name = "Fecha Factura")]
        public DateTime? Fec_factura { get; set; }

        [Display(Name = "Fecha Ingreso")]
        public DateTime? Fec_ingreso { get; set; }

        public string Proveedor { get; set; }

        public string Propietario { get; set; }
        public  CANAL CANAL { get; set; }      
        public  List<POS> POS { get; set; }
        public  List<SIM> SIM { get; set; }

    }
}