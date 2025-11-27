using CN.Lototo.Domain.Entities;
using CN.Lototo.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CN.Lototo.Infrastructure.Data
{
    public class LototoContext : DbContext
    {
        public LototoContext(DbContextOptions<LototoContext> options)
       : base(options)
        {
        }

        public DbSet<Plantas> Plantas => Set<Plantas>();
        public DbSet<Usuarios> Usuarios => Set<Usuarios>();
        public DbSet<Equipamento> Equipamentos => Set<Equipamento>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PlantaConfiguration());
            modelBuilder.ApplyConfiguration(new EquipamentoConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
