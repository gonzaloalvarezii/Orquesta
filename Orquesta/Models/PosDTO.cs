using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orquesta.Models
{
    public class PosDTO : ARTICULO
    {
        public string Nro_serie { get; set; }
        public int Id_Modelo { get; set; }
        public string SW_version { get; set; }
        public string Pk_version { get; set; }
      
    }
}