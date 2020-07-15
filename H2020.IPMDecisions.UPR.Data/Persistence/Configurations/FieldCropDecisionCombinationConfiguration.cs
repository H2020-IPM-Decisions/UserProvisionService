using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FieldCropDecisionCombinationConfiguration : IEntityTypeConfiguration<FieldCropDecisionCombination>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<FieldCropDecisionCombination> builder)
        {
            builder.HasKey(fcd =>
                 new
                 {
                     fcd.FielId,
                     fcd.CropDecisionCombinationId
                 });

            builder.HasOne<Field>(fcd => fcd.Field)
                .WithMany(f => f.FieldCropDecisionCombinations)
                .HasForeignKey(fcd => fcd.FielId)
                .HasConstraintName("FK_FieldCropDecision_Field")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne<CropDecisionCombination>(fcd => fcd.CropDecisionCombination)
                .WithMany(f => f.FieldCropDecisionCombinations)
                .HasForeignKey(uf => uf.CropDecisionCombinationId)
                .HasConstraintName("FK_FieldCropDecision_CropDecision")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}