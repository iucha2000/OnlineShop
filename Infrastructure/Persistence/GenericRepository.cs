using Application.Common.Persistence;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbSet = dbContext.Set<T>();
        }

        public async Task<Result> AddAsync(T entity)
        {
            var value = await _dbSet.AddAsync(entity);
            return Result.Succeed();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<Result> DeleteAsync(T entity)
        {
            var value = _dbSet.Remove(entity);
            return Result.Succeed();
           
        }

        public async Task<Result> DeleteByIdAsync(Guid Id)
        {
            var entity = await _dbSet.FindAsync(Id);
            if(entity != null)
            {
                return await DeleteAsync(entity);
            }
            return Result.Fail("Not found", 404);
        }

        public async Task<T> GetByExpressionAsync(Expression<Func<T, bool>> expression, string includes = null, bool trackChanges = true)
        {
            IQueryable<T> query = _dbSet;

            var includesList = includes?.Split(", ");
            if (includesList != null)
            {
                query = includesList.Aggregate(query, (current, includesList) => current.Include(includesList));
            }
            var result = trackChanges ? await query.FirstOrDefaultAsync(expression) : await query.AsNoTracking().FirstOrDefaultAsync(expression);
            return result;
        }

        public async Task<Result<T>> GetByIdAsync(Guid Id)
        {
            var result = await _dbSet.FindAsync(Id);
            return Result<T>.Succeed(result);
        }

        public async Task<IList<T>> ListAsync(Expression<Func<T, bool>> expression = null, string includes = null, bool trackChanges = true, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int count = 0)
        {
            IQueryable<T> query = _dbSet;

            var includesList = includes?.Split(", ");
            if(includesList != null)
            {
                query = includesList.Aggregate(query, (current,includesList) => current.Include(includesList));
            }
            if(expression != null)
            {
                query = query.Where(expression);
            }
            if(orderBy != null)
            {
                query = orderBy(query);
            }
            if(count > 0) 
            {
                query = query.Take(count);
            }
            var result = trackChanges ? await query.ToListAsync() : await query.AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<Result<T>> UpdateAsync(T entity)
        {
            var result = _dbSet.Update(entity);
            return Result<T>.Succeed(entity);
        }
    }
}
