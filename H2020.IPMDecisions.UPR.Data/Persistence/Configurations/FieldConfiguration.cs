using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FieldConfiguration : IEntityTypeConfiguration<Field>
    {
        public void Configure(EntityTypeBuilder<Field> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Property(f => f.Name)
                .IsRequired();

            builder.HasMany<FieldObservation>(f => f.FieldObservations)
                .WithOne(fo => fo.Field)
                .HasForeignKey(f => f.FieldId)
                .HasConstraintName("FK_Observation_Field")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}