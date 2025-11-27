using CN.Lototo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CN.Lototo.Infrastructure.Data.Configurations
{
    public class PlantaConfiguration : IEntityTypeConfiguration<Plantas>
    {
        public void Configure(EntityTypeBuilder<Plantas> builder)
        {
            builder.ToTable("Plantas");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Codigo)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Localizacao)
                .HasMaxLength(200);

            builder.Property(p => p.Ativa)
                .HasDefaultValue(true);

            builder.Property(p => p.DataCriacao)
                .IsRequired();

            builder.Property(p => p.DataAtualizacao)
                .IsRequired(false);

            builder.HasMany(p => p.Usuarios)
                .WithOne(u => u.Planta)
                .HasForeignKey(u => u.PlantaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Equipamentos)
                .WithOne(e => e.Planta)
                .HasForeignKey(e => e.PlantaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
