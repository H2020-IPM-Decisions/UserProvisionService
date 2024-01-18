using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IRepositoryBase<T>
    {
        void Create(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> FindAllAsync();
        Task<T> FindByConditionAsync(Expression<Func<T, bool>> expression);
        Task<T> FindByConditionAsync(Expression<Func<T, bool>> expression, bool includeAssociatedData);
        Task<T> FindByIdAsync(Guid id);
        void Update(T entity);
    }
}