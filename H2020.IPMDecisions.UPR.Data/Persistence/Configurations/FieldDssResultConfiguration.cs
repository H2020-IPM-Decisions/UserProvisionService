using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FieldDssResultConfiguration : IEntityTypeConfiguration<FieldDssResult>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<FieldDssResult> builder)
        {
            builder
                .Property(f => f.Result)
                .HasColumnType("jsonb");
        }
    }
}