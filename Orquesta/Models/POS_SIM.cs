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
    
    public partial class POS_SIM
    {
        public int Id { get; set; }
        public int Id_POS { get; set; }
        public int Id_SIM { get; set; }
    
        public virtual ARTICULO ARTICULO { get; set; }
        public virtual ARTICULO ARTICULO1 { get; set; }
    }
}
