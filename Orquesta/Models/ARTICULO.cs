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
    
    public partial class ARTICULO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ARTICULO()
        {
            this.DETALLE_INVENTARIO = new HashSet<DETALLE_INVENTARIO>();
            this.DETALLE_PEDIDO = new HashSet<DETALLE_PEDIDO>();
            this.POS_SIM = new HashSet<POS_SIM>();
            this.POS_SIM1 = new HashSet<POS_SIM>();
        }
    
        public int Id_Articulo { get; set; }
        public string Descripcion { get; set; }
        public Nullable<decimal> Precio_Venta { get; set; }
        public Nullable<decimal> Precio_Compra { get; set; }
        public string Stock { get; set; }
        public Nullable<int> Id_Categoria { get; set; }
        public Nullable<int> Id_Estado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DETALLE_INVENTARIO> DETALLE_INVENTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DETALLE_PEDIDO> DETALLE_PEDIDO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<POS_SIM> POS_SIM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<POS_SIM> POS_SIM1 { get; set; }
        public virtual ESTADO ESTADO { get; set; }
    }
}
