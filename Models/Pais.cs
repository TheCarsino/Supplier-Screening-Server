using System.ComponentModel.DataAnnotations;

namespace Supplier_Screening_Server.Models
{
    public class Pais
    {
        [Key]
        [MaxLength(3)]
        public string Codigo { get; set; }  // Código alpha-3

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        public decimal CodigoNum { get; set; }

        public ICollection<Proveedor> Proveedores { get; set; }

    }
}
