using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Helpers;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IRepositoryBase<T, Y>
    {
        void Create(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> FindAllAsync();
        Task<PagedList<T>> FindAllAsync(Y resourceParameter);
        Task<T> FindByCondition(Expression<Func<T, bool>> expression);
        Task<T> FindByCondition(Expression<Func<T, bool>> expression, bool includeAssociatedData);
        Task<T> FindByIdAsync(Guid id);
        void Update(T entity);
    }
}