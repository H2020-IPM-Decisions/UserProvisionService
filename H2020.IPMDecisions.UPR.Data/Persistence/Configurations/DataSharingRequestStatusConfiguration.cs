using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class DataSharingRequestStatusConfiguration : IEntityTypeConfiguration<DataSharingRequestStatus>
    {
        public void Configure(EntityTypeBuilder<DataSharingRequestStatus> builder)
        {
            builder.Property(d => d.Id)
                .ValueGeneratedNever();

            builder.HasIndex(d => d.Description)
                .IsUnique();

            builder.HasMany<DataSharingRequest>(d => d.DataSharingRequests)
                .WithOne(u => u.RequestStatus)
                .HasPrincipalKey(u => u.Description)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_DataSharingRequest_RequestStatus_RequestDescription")
                .IsRequired();
        }
    }
}