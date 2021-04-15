using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FieldDssObservationConfiguration : IEntityTypeConfiguration<FieldDssObservation>
    {
        public void Configure(EntityTypeBuilder<FieldDssObservation> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Property(f => f.Time)
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            builder
                .Property(f => f.DssObservation)
                .HasColumnType("jsonb");
        }
    }
}