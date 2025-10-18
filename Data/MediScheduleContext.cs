using MediSchedule_Prototipo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MediSchedule_Prototipo.Data
{
    
    /// Contexto principal de la base de datos MediSchedule.
    /// Gestiona las tablas y sus relaciones.
  
    public class MediScheduleContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<PuestoSalud> PuestosSalud { get; set; }
        public DbSet<Cita> Citas { get; set; }


        // Constructor — recibe opciones de configuración (por ejemplo, tipo de DB)
        public MediScheduleContext(DbContextOptions<MediScheduleContext> options)
            : base(options)
        {
        }

        // Configuración directa de la base si no se usa inyección de dependencias (consola)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Aquí se crea el archivo físico SQLite
                optionsBuilder.UseSqlite("Data Source=medischedule.db");
            }
        }

        // Configuraciones adicionales (relaciones, nombres de tabla, etc.)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.PuestoSalud)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(u => u.PuestoSaludId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Nombres de tabla más limpios
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<PuestoSalud>().ToTable("PuestosSalud");
        }
    }
}

