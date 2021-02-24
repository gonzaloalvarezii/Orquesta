using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orquesta.Models
{
    public class getPedidos
    {
        public int Id_Pedido { get; set; }
        public string canal { get; set; }
        public string usuario { get; set; }
        public System.DateTime Fecha_Ingreso { get; set; }
        public Nullable<System.DateTime> Fecha_Entrega { get; set; }
        public string Comentario { get; set; }
    }
}