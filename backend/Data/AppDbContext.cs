using Microsoft.EntityFrameworkCore;
using Adres.Prueba.Api.Models;

namespace Adres.Prueba.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Acquisition> Acquisitions => Set<Acquisition>();
    public DbSet<AcquisitionHistory> Histories => Set<AcquisitionHistory>();
    public DbSet<UnidadAdministrativa> UnidadesAdministrativas => Set<UnidadAdministrativa>();
    public DbSet<TipoBienServicio> TiposBienServicio => Set<TipoBienServicio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Decimal precision
        modelBuilder.Entity<Acquisition>(entity =>
        {
            entity.Property(e => e.Presupuesto).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ValorUnitario).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ValorTotal).HasColumnType("decimal(18,2)");
        });

        // Seed catálogos
        modelBuilder.Entity<UnidadAdministrativa>().HasData(
            new UnidadAdministrativa { Id = 1, Nombre = "Dirección General" },
            new UnidadAdministrativa { Id = 2, Nombre = "Subdirección de Gestión Financiera" },
            new UnidadAdministrativa { Id = 3, Nombre = "Oficina Asesora Jurídica" },
            new UnidadAdministrativa { Id = 4, Nombre = "Oficina de Tecnologías de la Información" },
            new UnidadAdministrativa { Id = 5, Nombre = "Subdirección de Aseguramiento" },
            new UnidadAdministrativa { Id = 6, Nombre = "Subdirección de Operación de Reconocimientos" },
            new UnidadAdministrativa { Id = 7, Nombre = "Oficina de Planeación" },
            new UnidadAdministrativa { Id = 8, Nombre = "Oficina de Control Interno" }
        );

        modelBuilder.Entity<TipoBienServicio>().HasData(
            new TipoBienServicio { Id = 1, Nombre = "Medicamentos" },
            new TipoBienServicio { Id = 2, Nombre = "Dispositivos médicos" },
            new TipoBienServicio { Id = 3, Nombre = "Equipos biomédicos" },
            new TipoBienServicio { Id = 4, Nombre = "Servicios de tecnología" },
            new TipoBienServicio { Id = 5, Nombre = "Servicios de consultoría" },
            new TipoBienServicio { Id = 6, Nombre = "Servicios de mantenimiento" },
            new TipoBienServicio { Id = 7, Nombre = "Papelería y suministros" },
            new TipoBienServicio { Id = 8, Nombre = "Servicios logísticos" },
            new TipoBienServicio { Id = 9, Nombre = "Licencias de software" },
            new TipoBienServicio { Id = 10, Nombre = "Servicios de capacitación" }
        );
    }
}
