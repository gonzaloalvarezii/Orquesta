using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orquesta.Models
{
    public class Detalle_InventarioDTO
    {
        public int Id_Inventario { get; set; }
        public int Id_Detalle_Inventario { get; set; }

        [Display(Name = "Codigo Artículo")]
        public int Id_articulo { get; set; }
        public Nullable<int> Id_Categoria { get; set; }

        [Display(Name = "Tipo Artículo")]
        public string Categoria { get; set; }

        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; }
        public string Estado { get; set; }


    }
}