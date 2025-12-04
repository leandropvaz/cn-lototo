using CN.Lototo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CN.Lototo.Infrastructure.Data.Configurations
{
    public class EquipamentoConfiguration : IEntityTypeConfiguration<Equipamento>
    {
        public void Configure(EntityTypeBuilder<Equipamento> builder)
        {
            builder.ToTable("Equipamento");

            builder.HasKey(e => e.Id);

            // CABEÇALHO
            builder.Property(e => e.Tag)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(e => e.EquipmentName)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(e => e.FactoryName)
                .HasColumnType("nvarchar(max)")   // fábrica | ex.: "Matozinhos"
                .IsRequired(false);

            builder.Property(e => e.RevisionInfo)
                .HasColumnType("nvarchar(max)")   // revisão | ex.: "Revisão 2 - 2024"
                .IsRequired(false);

            // IDENTIFICAÇÃO — Grupo I
            builder.Property(e => e.EnergyType)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(e => e.HazardDescription)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            // CONTROLE DE ENERGIA — Grupo II
            builder.Property(e => e.IsolationDeviceTag)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property(e => e.IsolationDeviceLocation)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property(e => e.IsolationDeviceDescription)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property(e => e.LockoutType)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);
            builder.Property(e => e.ZeroEnergyVerification)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);
            builder.Property(e => e.Test)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            // BLOBs
            builder.Property(e => e.SourceExcelBlobUrl)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.ImageBlobUrl)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.ImageNotes)
                .HasColumnType("nvarchar(max)");

            // FLAGS E AUDITORIA
            builder.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
