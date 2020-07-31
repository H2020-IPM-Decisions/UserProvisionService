using System;
using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class UserFarmTypeConfiguration : IEntityTypeConfiguration<UserFarmType>
    {
        public void Configure(EntityTypeBuilder<UserFarmType> builder)
        {
            builder.Property(u => u.Id)
                .ValueGeneratedNever();

            builder.HasIndex(u => u.Description)
                .IsUnique();

            builder.HasMany<UserFarm>(u => u.UserFarms)
                .WithOne(uf => uf.UserFarmType)
                .HasPrincipalKey(u => u.Description)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}