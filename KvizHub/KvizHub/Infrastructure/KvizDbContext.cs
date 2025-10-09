using KvizHub.Models;
using Microsoft.EntityFrameworkCore;

namespace KvizHub.Infrastructure
{
    public class KvizDbContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public KvizDbContext(DbContextOptions options) : base(options) 
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(KvizDbContext).Assembly);
        }
    }
}
