using Model;

using Microsoft.EntityFrameworkCore;

namespace Infra.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>()
                .Property(p => p.Name).HasMaxLength(10).IsRequired();

            builder.Entity<Category>()
                .ToTable("Categories");
            builder.Entity<Tag>()
                .ToTable("Tags");
        }
    }
}