using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Core
{
    public interface IApplicationDbContext
    {
        DbSet<UserProfile> UserProfile { get; set; }
    }
}