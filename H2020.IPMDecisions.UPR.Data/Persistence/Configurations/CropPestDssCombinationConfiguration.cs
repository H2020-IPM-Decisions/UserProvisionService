using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class CropPestDssCombinationConfiguration : IEntityTypeConfiguration<CropPestDssCombination>
    {
        public void Configure(EntityTypeBuilder<CropPestDssCombination> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(
                    c => new
                    {
                        c.CropPestId,
                        c.DssName
                    })
                .IsUnique();

            builder.HasOne<CropPest>(c => c.CropPest)
                .WithMany(cp => cp.CropPestDssCombinations)
                .HasForeignKey(c => c.CropPestId)
                .HasConstraintName("FK_CropPestDss_Combination")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}