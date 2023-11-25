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
    internal class MockOrderRepository : IGenericRepository<Order>
    {
        private Dictionary<Guid, Order> _dataBase;

        public MockOrderRepository(MockDb dataBase)
        {
            _dataBase = dataBase.Orders;
        }

        public async Task<Result> AddAsync(Order entity)
        {
            await Task.CompletedTask;
            entity.Id = Guid.NewGuid();
            _dataBase.TryAdd(entity.Id, entity);
            return Result.Succeed();
        }

        public Task AddRangeAsync(IEnumerable<Order> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteAsync(Order entity)
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

        public Task<Order> GetByExpressionAsync(Expression<Func<Order, bool>> expression, string includes = null, bool trackChanges = false)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Order>> GetByIdAsync(Guid Id)
        {
            await Task.CompletedTask;
            if (_dataBase.TryGetValue(Id, out var result))
            {
                return Result<Order>.Succeed(result);
            }
            else
            {
                return Result<Order>.Fail("Not Found", StatusCodes.Status404NotFound);
            }
        }

        public Task<IList<Order>> ListAsync(Expression<Func<Order, bool>> expression, string includes = null, bool trackChanges = false, Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy = null, int count = 0)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Order>> UpdateAsync(Order entity)
        {
            throw new NotImplementedException();
        }
    }
}
