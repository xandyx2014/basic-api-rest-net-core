using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AutorLibro>()
                .HasKey( e => new { e.AutorId, e.LibroId });
        }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libro { get; set; }
        public DbSet<Comentario> Comentario { get; set; }
        public DbSet<AutorLibro> AutorLibro { get; set; }
    }
}
