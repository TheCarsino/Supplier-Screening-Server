using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Supplier_Screening_Server.Models
{
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string RazonSocial { get; set; }

        [Required]
        [MaxLength(255)]
        public string NombreComercial { get; set; }

        [Required]
        [Length(11,11)]
        public string IdentificacionTributaria { get; set; }
        
        [MaxLength(15)]
        public string? NumeroTelefonico { get; set; }

        
        [MaxLength(255)]
        public string? CorreoElectronico { get; set; }

        
        [MaxLength(255)]
        public string? SitioWeb { get; set; }

        [Required]
        [MaxLength(255)]
        public string DireccionFisica { get; set; }

        [Required]
        [MaxLength(3)]
        [Column("Pais")]
        public string PaisCodigo { get; set; }

        [ForeignKey("PaisCodigo")]
        public virtual Pais? Pais { get; set; }  // Navigation to Pais Model

        
        [Column(TypeName = "money")]
        public decimal? FacturacionAnual { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime FechaUltimaEdicion { get; set; } = DateTime.Now;

        [Required]
        public bool Activo { get; set; } = true;

    }

    public class ModProveedor
    {
        [Required]
        [MaxLength(255)]
        public string RazonSocial { get; set; }

        [Required]
        [MaxLength(255)]
        public string NombreComercial { get; set; }

        [Required]
        [Length(11, 11)]
        public string IdentificacionTributaria { get; set; }


        [MaxLength(15)]
        public string? NumeroTelefonico { get; set; }


        [MaxLength(255)]
        public string? CorreoElectronico { get; set; }


        [MaxLength(255)]
        public string? SitioWeb { get; set; }

        [Required]
        [MaxLength(255)]
        public string DireccionFisica { get; set; }

        [Required]
        [MaxLength(3)]
        [Column("Pais")]
        public string PaisCodigo { get; set; }

        [Column(TypeName = "money")]
        public decimal? FacturacionAnual { get; set; }

    }
}
