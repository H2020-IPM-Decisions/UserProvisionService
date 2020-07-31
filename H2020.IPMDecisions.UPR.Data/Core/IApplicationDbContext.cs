using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Core
{
    public interface IApplicationDbContext
    {
        DbSet<CropDecisionCombination> CropDecisionCombination { get; set; }
        DbSet<Farm> Farm { get; set; }
        DbSet<Field> Field { get; set; }
        DbSet<FieldObservation> FieldObservation { get; set; }
        DbSet<UserAddress> UserAddress { get; set; }
        DbSet<UserFarm> UserFarm { get; set; }
        DbSet<UserFarmType> UserFarmType { get; set; }
        DbSet<UserProfile> UserProfile { get; set; }
    }
}