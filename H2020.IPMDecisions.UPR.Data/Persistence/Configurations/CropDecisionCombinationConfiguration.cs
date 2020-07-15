using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class CropDecisionCombinationConfiguration : IEntityTypeConfiguration<CropDecisionCombination>
    {
        public void Configure(EntityTypeBuilder<CropDecisionCombination> builder)
        {
            builder.HasKey(c =>
                new
                {
                    c.CropId,
                    c.DssId,
                    c.PestId,
                });

            builder.HasOne<Crop>(c => c.Crop)
                .WithMany(cr => cr.CropDecisionCombinations)
                .HasForeignKey(c => c.CropId)
                .HasConstraintName("FK_CropCombination_Crop")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.HasOne<Dss>(c => c.Dss)
                .WithMany(cr => cr.CropDecisionCombinations)
                .HasForeignKey(c => c.DssId)
                .HasConstraintName("FK_CropCombination_Dss")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.HasOne<Pest>(c => c.Pest)
                .WithMany(cr => cr.CropDecisionCombinations)
                .HasForeignKey(c => c.PestId)
                .HasConstraintName("FK_CropCombination_Pest")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}