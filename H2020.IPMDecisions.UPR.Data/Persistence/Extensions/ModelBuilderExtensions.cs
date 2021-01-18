using System;
using System.Collections.Generic;
using System.Linq;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFarmType>()
            .HasData(
                Enum.GetValues(typeof(UserFarmTypeEnum))
                .Cast<UserFarmTypeEnum>()
                .Select(e => new UserFarmType()
                {
                    Id = e,
                    Description = e.ToString()
                })
            );

            modelBuilder.Entity<DataSharingRequestStatus>()
            .HasData(
                Enum.GetValues(typeof(RequestStatusEnum))
                .Cast<RequestStatusEnum>()
                .Select(e => new DataSharingRequestStatus()
                {
                    Id = e,
                    Description = e.ToString()
                })
            );

            modelBuilder.Entity<Widget>()
            .HasData(
                Enum.GetValues(typeof(WidgetOption))
                .Cast<WidgetOption>()
                .Select(e => new Widget()
                {
                    Id = e,
                    Description = e.ToString()
                })
            );
        }
    }
}