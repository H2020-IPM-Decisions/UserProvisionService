using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class UserFarmConfiguration : IEntityTypeConfiguration<UserFarm>
    {
        public void Configure(EntityTypeBuilder<UserFarm> builder)
        {
            builder.HasKey(uf => 
                new {
                    uf.UserId,
                    uf.FarmId
                });

            builder.HasOne<UserProfile>(uf => uf.UserProfile)
                .WithMany(u => u.UserFarms)
                .HasForeignKey(uf => uf.UserId)
                .HasConstraintName("FK_UserFarm_User")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne<Farm>(uf => uf.Farm)
                .WithMany(f => f.UserFarms)
                .HasForeignKey(uf => uf.FarmId)
                .HasConstraintName("FK_UserFarm_Farm")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(uf => uf.Authorised)
                .IsRequired();
        }
    }
}