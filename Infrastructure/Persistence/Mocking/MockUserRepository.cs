using Application.Common.Persistence;
using Domain;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Mocking
{
    internal class MockUserRepository: IGenericRepository<User>
    {
        private Dictionary<Guid, User> _dataBase;

        public MockUserRepository(MockDb dataBase)
        {
            _dataBase = dataBase.Users;
        }

        public async Task<Result> AddAsync(User entity)
        {
            await Task.CompletedTask;
            entity.Id = Guid.NewGuid();
            _dataBase.TryAdd(entity.Id, entity);
            return Result.Succeed();
        }

        public async Task AddRangeAsync(IEnumerable<User> entities)
        {
            foreach (var entity in entities)
            {
                await AddAsync(entity);
            }
        }

        public async Task<Result> DeleteAsync(User entity)
        {
            var result = await DeleteByIdAsync(entity.Id);
            return result;
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

        public async Task<User> GetByExpressionAsync(Expression<Func<User, bool>> expression, string includes = null, bool trackChanges = false)
        {
            await Task.CompletedTask;
            return _dataBase.Values.AsQueryable().Where(expression).FirstOrDefault();
        }

        public async Task<Result<User>> GetByIdAsync(Guid Id)
        {
            await Task.CompletedTask;
            if (_dataBase.TryGetValue(Id, out var result))
            {
                return Result<User>.Succeed(result);
            }
            else
            {
                return Result<User>.Fail("Not Found", StatusCodes.Status404NotFound);
            }
        }

        public async Task<IList<User>> ListAsync(Expression<Func<User, bool>> expression, string includes = null, bool trackChanges = false, Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null, int count = 0)
        {
            await Task.CompletedTask;
            return _dataBase.Values.AsQueryable().Where(expression).ToList();
        }

        public async Task<Result<User>> UpdateAsync(User entity)
        {
            await Task.CompletedTask;
            if (_dataBase.TryGetValue(entity.Id, out var result))
            {
                result.Email = entity.Email;
                result.PasswordSalt = entity.PasswordSalt;
                result.PasswordHash = entity.PasswordHash;
                result.Balance = entity.Balance;
                result.Orders = entity.Orders;
            }

            return Result<User>.Succeed(entity);
        }
    }
}
