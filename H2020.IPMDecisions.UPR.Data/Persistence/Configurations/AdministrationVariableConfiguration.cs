using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class AdministrationVariableConfiguration : IEntityTypeConfiguration<AdministrationVariable>
    {
        public void Configure(EntityTypeBuilder<AdministrationVariable> builder)
        {
            builder.Property(a => a.Id)
               .ValueGeneratedNever();

            builder.HasIndex(a => a.Description)
                .IsUnique();

            builder.Property(a => a.Description)
               .IsRequired();
        }
    }
}