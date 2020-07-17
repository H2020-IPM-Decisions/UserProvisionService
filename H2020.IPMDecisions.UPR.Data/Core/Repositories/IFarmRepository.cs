using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IFarmRepository : IRepositoryBase<Farm, FarmResourceParameter>
    {
    }
}