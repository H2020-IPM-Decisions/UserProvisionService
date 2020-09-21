
using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(u => u.UserId);

            builder.Property(u => u.UserId)
                .ValueGeneratedNever();

            builder.Property(u => u.FirstName)
                .IsRequired();

            builder.HasOne(u => u.UserAddress)
                .WithMany()
                .HasForeignKey(e => e.UserAddressId)
                .HasConstraintName("FK_User_UserAddress")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<DataSharingRequest>(u => u.DataSharingRequests)
                .WithOne(d => d.Requestee)
                .HasForeignKey(u => u.RequesteeId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}