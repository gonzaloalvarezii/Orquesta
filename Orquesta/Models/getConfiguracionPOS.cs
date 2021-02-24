using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orquesta.Models
{
    public class getConfiguracionPOS
    {
        public int Id_Pedido { get; set; }
        public string Nro_serie_POS { get; set; }
        public string Modelo { get; set; }
        public string Software { get; set; }
        public string Package { get; set; }
        public string Nro_serie_SIM { get; set; }
        public string Descripcion { get; set; }
    }
}