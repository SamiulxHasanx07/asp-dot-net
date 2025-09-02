using Microsoft.EntityFrameworkCore;
using Models;

namespace Helpers
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }
    public DbSet<Models.User> Users { get; set; }
    public DbSet<Models.Order> Orders { get; set; }
    }
}
