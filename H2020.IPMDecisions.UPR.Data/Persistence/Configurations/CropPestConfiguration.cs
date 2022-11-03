using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class CropPestConfiguration : IEntityTypeConfiguration<CropPest>
    {
        public void Configure(EntityTypeBuilder<CropPest> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasIndex(
                    c => new { 
                        c.CropEppoCode, 
                        c.PestEppoCode 
                    })
                .IsUnique();
        }
    }
}