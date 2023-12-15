using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        
        }

        DbSet<Order> Orders { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<Image> Images { get; set; }
        DbSet<Comment> Comments { get; set; }
    }
}
