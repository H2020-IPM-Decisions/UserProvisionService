using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class EppoCodeConfiguration : IEntityTypeConfiguration<EppoCode>
    {
        public void Configure(EntityTypeBuilder<EppoCode> builder)
        {
            builder
                .HasIndex(e => e.Type)
                .IsUnique();

            builder.Property(e => e.Data)
                .HasColumnType("jsonb")
                .IsRequired();
        }
    }
}