using Microsoft.EntityFrameworkCore;
using Supplier_Screening_Server.Models;

namespace Supplier_Screening_Server.Context
{
    public class APIDBContext : DbContext
    {
        /*Constructor*/
        public APIDBContext(DbContextOptions<APIDBContext> options): base(options)
        {

        }

        public DbSet<UsuarioEY> Usuarios { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Proveedor> Proveedores{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Proveedor>()
                .HasOne(p => p.Pais)
                .WithMany(p => p.Proveedores)
                .HasForeignKey(p => p.PaisCodigo)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }


    }
}
