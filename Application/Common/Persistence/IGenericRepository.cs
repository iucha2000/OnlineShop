using Domain;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Persistence
{
    public interface IGenericRepository <T> where T : BaseEntity
    {
        Task<Result> AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task<Result> DeleteAsync(T entity);

        Task<Result> DeleteByIdAsync(Guid Id);

        Task<Result<T>> UpdateAsync(T entity);

        Task<Result<T>> GetByIdAsync(Guid Id);

        Task<T> GetByExpressionAsync(Expression<Func<T, bool>> expression, string includes = null, bool trackChanges = false);

        Task<IList<T>> ListAsync(Expression<Func<T, bool>> expression, string includes = null, bool trackChanges = false, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int count = 0);

    }
}
