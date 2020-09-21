using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FieldObservationConfiguration : IEntityTypeConfiguration<FieldObservation>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<FieldObservation> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Property(f => f.Location)
                .HasColumnType("geometry (point)")
                .IsRequired();

            builder.Property(f => f.Time)
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            builder.Property(f => f.CropEppoCode)
                .IsRequired()
                .HasMaxLength(6);
            
            builder.Property(f => f.PestEppoCode)
                .IsRequired()
                .HasMaxLength(6);
        }
    }
}