using GoogleRuta.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GoogleRuta.Data
{

    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<CoordinateB> CoordinateBs { get; set; }
        public DbSet<ElementProject> ElementProjects { get; set; }
        public DbSet<ElementType> ElementTypes { get; set; }
        public DbSet<ColorTraces> ColorTraces { get; set; }
        public DbSet<ColorThreadProject> ColorThreadProjects { get; set; }
        public DbSet<Nodo> Nodos { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Drawing> Drawings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Relación Project a CoordinateB: Cuando borras un Project, borra sus CoordinateBs.
            modelBuilder.Entity<Project>()
                  .HasMany(p => p.CoordinateBs)
                  .WithOne(c => c.Project)
                  .HasForeignKey(c => c.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            //Relacion de project a ElementProject : cuando borras un project borra sus elementProject       
            modelBuilder.Entity<Project>()
                        .HasMany(p => p.ElementProjects)
                        .WithOne(ep => ep.Project)
                        .HasForeignKey(ep => ep.ProjectId)
                        .OnDelete(DeleteBehavior.Cascade);
            //relacion elementType a ElementProject cuando borras un elementtYPE, el elementTypeId en elementProject se vuelve null

            modelBuilder.Entity<ElementType>()
            .HasMany(et => et.ElementProject)
            .WithOne(ep => ep.ElementType)
            .HasForeignKey(ep => ep.ElementTypeId)
            .IsRequired(false)// Esto es importante para permitir que ElementTypeId sea NULL
            .OnDelete(DeleteBehavior.SetNull);// ¡CAMBIO CLAVE AQUÍ!: Ahora es SET NULL

            //hasMany:un proyecto tiene muchos colorThe...
            //withOne: y cada colorthe.. esta relacionado con un solo project
            //hasForeignKey:la columan que conecta estas dos tablas es ProjectId
            //onDelete: si un registro de la tabla project es elimniado automaticamente busca y elimina todos
            //los registro en la tabla colorTheadProject que esten vinculados a el (que tengan el mismo ProjectId)
            modelBuilder.Entity<Project>()
            .HasMany(p => p.ColorThreadProjects)
            .WithOne(ep => ep.Project)
            .HasForeignKey(ep => ep.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

            // Configuración para la relación entre ColorTraces y ColorThreadProject
            modelBuilder.Entity<ColorTraces>()
                .HasMany(ct => ct.ColorThreadProjects)       // Un ColorTraces tiene muchas ColorThreadProjects (reglas)
                .WithOne(ctp => ctp.ColorTraces)             // Una regla tiene un solo ColorTraces
                .HasForeignKey(ctp => ctp.ColorTracesId)     // La clave foránea es ColorTracesId
                .IsRequired(false)                           // La relación no es obligatoria (una regla puede no tener color)
                .OnDelete(DeleteBehavior.SetNull);           // ¡LA REGLA CLAVE! Si borras un color del catálogo,
                                                             // las reglas que lo usaban ahora tendrán su ColorTracesId = NULL.


            // // 4. Relación CoordinateB a ElementProject: Cuando borras una CoordinateB, no permitas si tiene ElementProjects asociados.
            // //    Esto es crucial para romper el ciclo de cascadas.
            // modelBuilder.Entity<CoordinateB>()
            //     .HasMany(cb => cb.ElementProjects)
            //     .WithOne(ep => ep.CoordinateB)
            //     .HasForeignKey(ep => ep.CoordinateBId)
            //     .OnDelete(DeleteBehavior.Restrict);


            // Agrega esto al final de tu método OnModelCreating
            // ---
            // Configuración para la relación de Origen (OriginNodoId)
            modelBuilder.Entity<Connection>()
                .HasOne(c => c.OriginNodo) // Una conexión tiene un nodo de origen
                .WithMany()                // Un nodo puede ser el origen de muchas conexiones
                .HasForeignKey(c => c.OrigenNodoId)
                .OnDelete(DeleteBehavior.Restrict); // ¡Importante! No borrar en cascada

            // Configuración para la relación de Destino (DestinationNodoId)
            modelBuilder.Entity<Connection>()
                .HasOne(c => c.DestinationNodo) // Una conexión tiene un nodo de destino
                .WithMany()                    // Un nodo puede ser el destino de muchas conexiones
                .HasForeignKey(c => c.DestinationNodoId)
                .OnDelete(DeleteBehavior.Restrict); // ¡Importante! No borrar en cascada

        }
    }
}

