using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class ForecastResultConfiguration : IEntityTypeConfiguration<ForecastResult>
    {
        public void Configure(EntityTypeBuilder<ForecastResult> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(f =>
            new
            {
                f.ForecastAlertId,
                f.Date
            })
            .IsUnique();
        }
    }
}