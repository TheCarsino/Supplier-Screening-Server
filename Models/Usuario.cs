using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Supplier_Screening_Server.Models
{
    public class UsuarioEY
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Usuario { get; set; }

        [Required]
        [MaxLength(255)]
        public string HashContrasena { get; set; }

        [MaxLength(150)]
        public string? Nombre { get; set; }

        [MaxLength(150)]
        public string? Apellidos { get; set; }
    }

    public class UsuarioCredentials
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Usuario { get; set; }

        [Required]
        [MaxLength(50)]
        public string Contrasena { get; set; }

        [MaxLength(150)]
        public string? Nombre { get; set; }

        [MaxLength(150)]
        public string? Apellidos { get; set; }
    }

    public class AuthUsuario
    {
        [Required]
        public string Usuario { get; set; }

        [Required]
        public string Contrasena { get; set; }
    }

}
