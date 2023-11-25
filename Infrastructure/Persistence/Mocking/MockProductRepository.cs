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
    public class MockProductRepository: IGenericRepository<Product>
    {
        private Dictionary<Guid, Product> _dataBase;

        public MockProductRepository(MockDb dataBase)
        {
            _dataBase = dataBase.Products;
        }

        public async Task<Result> AddAsync(Product entity)
        {
            await Task.CompletedTask;
            entity.Id = Guid.NewGuid();
            _dataBase.TryAdd(entity.Id, entity);
            return Result.Succeed();
        }

        public async Task AddRangeAsync(IEnumerable<Product> entities)
        {
            foreach (var entity in entities)
            {
                await AddAsync(entity);
            }
        }

        public async Task<Result> DeleteAsync(Product entity)
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

        public async Task<Product> GetByExpressionAsync(Expression<Func<Product, bool>> expression, string includes = null, bool trackChanges = false)
        {
            await Task.CompletedTask;
            return _dataBase.Values.AsQueryable().Where(expression).FirstOrDefault();
        }

        public async Task<Result<Product>> GetByIdAsync(Guid Id)
        {
            await Task.CompletedTask;
            if (_dataBase.TryGetValue(Id, out var result))
            {
                return Result<Product>.Succeed(result);
            }
            else
            {
                return Result<Product>.Fail("Not Found", StatusCodes.Status404NotFound);
            }
        }

        public async Task<IList<Product>> ListAsync(Expression<Func<Product, bool>> expression, string includes = null, bool trackChanges = false, Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null, int count = 0)
        {
            await Task.CompletedTask;
            return _dataBase.Values.AsQueryable().Where(expression).ToList();
        }

        public async Task<Result<Product>> UpdateAsync(Product entity)
        {
            await Task.CompletedTask;
            if (_dataBase.TryGetValue(entity.Id, out var result))
            {
                result.Name = entity.Name;
                result.Price = entity.Price;
                result.Category = entity.Category;
                result.RemainingCount = entity.RemainingCount;
            }

            return Result<Product>.Succeed(entity);
        }
    }
}
