//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Orquesta.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SIM : ARTICULO
    {
        public string Nro_serie { get; set; }
        public string PIN { get; set; }
        public string Puk { get; set; }
        public string Phone_Numer { get; set; }
        public Nullable<int> Id_Tipo_Operador { get; set; }
    
        public virtual TIPO_OPERADOR TIPO_OPERADOR { get; set; }
    }
}
