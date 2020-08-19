using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface ICropPestRepository
    {
        void Create(CropPest entity);
        Task<CropPest> FindByConditionAsync(Expression<Func<CropPest, bool>> expression);
    }
}