using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RaZorWebxxx.Models.Blog;
using RaZorWebxxx.Models.Contacts;
using RaZorWebxxx.Models.Product;


namespace RaZorWebxxx.Models
{
    public class MyBlogContext : IdentityDbContext<AppUser>
    {
        public MyBlogContext(DbContextOptions<MyBlogContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });
            modelBuilder.Entity<Postcategory>(entity =>
            {
                entity.HasKey(c => new { c.PostID, c.CategoryID });
            });
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(p => p.Slug).IsUnique();
            });
            modelBuilder.Entity<Categoryproduct>(entity =>
                {
                    entity.HasIndex(c => c.Slug).IsUnique();
                });
            modelBuilder.Entity<ProductCategoryproduct>(entity =>
            {
                entity.HasKey(c => new { c.ProductID, c.CategoryID });
            });
            modelBuilder.Entity<ProducModel>(entity =>
            {
                entity.HasIndex(p => p.Slug).IsUnique();
            });

        }
    
        public DbSet<Contact> Contacts { set; get; }
        public DbSet<Category> Categories { set; get; }
        public DbSet<Post> Posts { set; get; }
    public DbSet<Postcategory> PostCategories { set; get; }
    public DbSet<Categoryproduct> Categoryproducts { set; get; }
    public DbSet<ProducModel> Products { set; get; }
    public DbSet<ProductCategoryproduct> ProductCategoryproducts { set; get; }
     public DbSet<ProductPhoto> ProductPhotos { set; get; }
 

    }
}
