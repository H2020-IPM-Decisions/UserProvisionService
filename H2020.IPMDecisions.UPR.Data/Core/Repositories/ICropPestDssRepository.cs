using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface ICropPestDssRepository
    {
        void Create(CropPestDss entity);
        Task<CropPestDss> FindByConditionAsync(Expression<Func<CropPestDss, bool>> expression);
        Task<List<CropPestDss>> FindAllAsync();
    }
}