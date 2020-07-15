using H2020.IPMDecisions.UPR.Core;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<CropDecisionCombination> CropDecisionCombination { get; set; }
        public DbSet<Farm> Farm { get; set; }
        public DbSet<Field> Field { get; set; }
        public DbSet<FieldObservation> FieldObservation { get; set; }
        public DbSet<UserAddress> UserAddress { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.ApplyConfiguration(new CropConfiguration());
            modelBuilder.ApplyConfiguration(new CropDecisionCombinationConfiguration());
            modelBuilder.ApplyConfiguration(new DssConfiguration());
            modelBuilder.ApplyConfiguration(new FarmConfiguration());
            modelBuilder.ApplyConfiguration(new FieldConfiguration());
            modelBuilder.ApplyConfiguration(new FieldObservationConfiguration());
            modelBuilder.ApplyConfiguration(new PestConfiguration());
            modelBuilder.ApplyConfiguration(new UserFarmConfiguration());
            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            modelBuilder.ApplyConfiguration(new UserAddressConfiguration());
        }
    }
}