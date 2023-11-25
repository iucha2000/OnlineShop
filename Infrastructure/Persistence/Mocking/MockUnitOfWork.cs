using Application.Common.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Mocking
{
    public class MockUnitOfWork : IUnitOfWork
    {
        public async Task<bool> CommitAsync()
        {
            await Task.CompletedTask; 
            return true;
        }
    }
}
