using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class DataSharingRequestConfiguration : IEntityTypeConfiguration<DataSharingRequest>
    {
        public void Configure(EntityTypeBuilder<DataSharingRequest> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(d =>
                new
                {
                    d.RequesteeId,
                    d.RequesterId
                })
                .IsUnique();
        }
    }
}