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
                    Enum.GetValues(typeof(UserFarmTypes))
                    .Cast<UserFarmTypes>()
                    .Select(e => new UserFarmType()
                    {
                        Id = e,
                        Description = e.ToString()
                    })
                );
        }        
    }
}