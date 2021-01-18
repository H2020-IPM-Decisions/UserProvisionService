using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class UserWidgetConfiguration : IEntityTypeConfiguration<UserWidget>
    {
        public void Configure(EntityTypeBuilder<UserWidget> builder)
        {
            builder.HasKey(uw =>
                new
                {
                    uw.UserId,
                    uw.WidgetId
                });

            builder.Property(uw => uw.Allowed)
                .IsRequired();

            builder.HasOne<UserProfile>(uw => uw.UserProfile)
                .WithMany(u => u.UserWidgets)
                .HasForeignKey(uw => uw.UserId)
                .HasConstraintName("FK_UserWidget_User")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne<Widget>(uw => uw.Widget)
                .WithMany(w => w.UserWidgets)
                .HasPrincipalKey(uw => uw.Description)
                .HasConstraintName("FK_UserWidget_Widget")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}