using Application.Common.Persistence;
using Domain;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Mocking
{
    internal class MockUserRepository<User> : IGenericRepository<User> where User : BaseEntity
    {
        private Dictionary<Guid, User> _dataBase;

        public MockUserRepository(MockDb dataBase)
        {
            //_dataBase = 
        }

        public async Task<Result> AddAsync(User entity)
        {
            await Task.CompletedTask; 
            _dataBase.Add(Guid.NewGuid(), entity);
            return Result.Succeed();
        }

        public Task AddRangeAsync(IEnumerable<User> entities)
        {
            foreach (var entity in entities)
            {
                _dataBase.Add(Guid.NewGuid(), entity);
            }
            return Task.CompletedTask;
        }

        public Task<Result> DeleteAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteByIdAsync(Guid Id)
        {
            await Task.CompletedTask;
            if (_dataBase.Remove(Id))
            {
                return Result.Succeed();
            }
            return Result.Fail("Not Found", 404);
        }

        public Task<User> GetByExpressionAsync(Expression<Func<User, bool>> expression, string includes = null, bool trackChanges = false)
        {
            throw new NotImplementedException();
        }

        public Task<Result<User>> GetByIdAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<User>> ListAsync(Expression<Func<User, bool>> expression, string includes = null, bool trackChanges = false, Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null, int count = 0)
        {
            throw new NotImplementedException();
        }

        public Task<Result<User>> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
