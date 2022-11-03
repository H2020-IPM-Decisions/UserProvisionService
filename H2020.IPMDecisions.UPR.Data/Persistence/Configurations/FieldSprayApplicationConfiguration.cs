using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FieldSprayApplicationConfiguration : IEntityTypeConfiguration<FieldSprayApplication>
    {
        public void Configure(EntityTypeBuilder<FieldSprayApplication> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.Time)
                .HasDefaultValueSql("NOW()")
                .IsRequired();
        }
    }
}