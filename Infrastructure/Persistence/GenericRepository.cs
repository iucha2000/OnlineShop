using Application.Common.Persistence;
using Domain;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        public Task<Result> AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteByIdAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByExpressionAsync(Expression<Func<T, bool>> expression, string includes = null, bool trackChanges = false)
        {
            throw new NotImplementedException();
        }

        public Task<Result<T>> GetByIdAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<T>> ListAsync(Expression<Func<T, bool>> expression, string includes = null, bool trackChanges = false, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int count = 0)
        {
            throw new NotImplementedException();
        }

        public Task<Result<T>> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
