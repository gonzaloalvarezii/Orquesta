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
    
    public partial class MODELO_PLAN
    {
        public int Id_Modelo_Plan { get; set; }
        public int Id_Modelo { get; set; }
        public int Id_Plan { get; set; }
        public System.DateTime Fecha_Asociado { get; set; }
    
        public virtual MODELO MODELO { get; set; }
        public virtual PLANES PLANES { get; set; }
    }
}
