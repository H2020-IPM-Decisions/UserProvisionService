
using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(u => u.UserId)
                .IsUnique();

            builder.Property(u => u.UserId)
                .IsRequired();

            builder.Property(u => u.UserId)
                .ValueGeneratedNever();

            builder.Property(u => u.FirstName)
                .IsRequired();

            builder.HasOne(u => u.UserAddress)
                .WithMany()
                .HasForeignKey(e => e.UserAddressId)
                .HasConstraintName("FK_User_UserAddress")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}