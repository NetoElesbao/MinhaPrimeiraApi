using Entity;
using Microsoft.EntityFrameworkCore;
namespace Settings
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder Builder)
        {
            Builder.Entity<Product>()
                .Property(p => p.Description).HasMaxLength(500).IsRequired(false);
            Builder.Entity<Product>()
                .Property(p => p.Name).HasMaxLength(120).IsRequired(true);
            Builder.Entity<Product>()
                .Property(p => p.Code).HasMaxLength(20).IsRequired(true);
            Builder.Entity<Category>()
                .ToTable("Categories");
        }
    }
}


