using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orquesta.Models
{
    public class Cliente
    {
       
        public Cliente() { }


        [Required]
        [StringLength(20)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(14)]
        public string RUT { get; set; }
    }
}