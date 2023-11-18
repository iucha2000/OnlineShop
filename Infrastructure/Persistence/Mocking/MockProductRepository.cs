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
    public class MockProductRepository<Product> : IGenericRepository<Product> where Product : BaseEntity
    {
        private Dictionary<Guid, Product> _dataBase;

        public MockProductRepository(MockDb dataBase)
        {
            //_dataBase = 
        }

        public Task<Result> AddAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<Product> entities)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteByIdAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetByExpressionAsync(Expression<Func<Product, bool>> expression, string includes = null, bool trackChanges = false)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Product>> GetByIdAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Product>> ListAsync(Expression<Func<Product, bool>> expression, string includes = null, bool trackChanges = false, Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null, int count = 0)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Product>> UpdateAsync(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
