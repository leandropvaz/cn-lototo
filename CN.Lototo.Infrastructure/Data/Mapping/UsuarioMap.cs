using CN.Lototo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CN.Lototo.Infrastructure.Data.Mapping
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuarios>
    {
        public void Configure(EntityTypeBuilder<Usuarios> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Login)
                .IsRequired()
                .HasMaxLength(60);

            builder.HasIndex(u => u.Login).IsUnique();

            builder.Property(u => u.NomeCompleto)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.SenhaHash)
                .IsRequired();

            builder.Property(u => u.Perfil)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne(u => u.Planta)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(u => u.PlantaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
