using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_barkery1.Models;

namespace Web_barkery1.data
{
    public class BakeryDbContext : IdentityDbContext<ApplicationUser>
    {
        public BakeryDbContext(
            DbContextOptions<BakeryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();

        public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();
        public DbSet<StoreInfo> StoreInfos => Set<StoreInfo>();

        public DbSet<ShopOrder> ShopOrders => Set<ShopOrder>();
        public DbSet<ShopOrderItem> ShopOrderItems => Set<ShopOrderItem>();
        public DbSet<ShopCartItem> ShopCartItems => Set<ShopCartItem>();
        public DbSet<StockBatch> StockBatches => Set<StockBatch>();
        public DbSet<OrderBatchAllocation> OrderBatchAllocations => Set<OrderBatchAllocation>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().Property(x => x.OldPrice).HasPrecision(18,2);
            modelBuilder.Entity<Product>().Property(x => x.Rating).HasPrecision(2,1);
            modelBuilder.Entity<ShopCartItem>().HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();
            modelBuilder.Entity<ShopOrder>().HasIndex(x => x.Code).IsUnique();
            modelBuilder.Entity<ShopOrder>().HasIndex(x => x.GuestTokenHash).IsUnique().HasFilter("[GuestTokenHash] IS NOT NULL");
            modelBuilder.Entity<ShopOrderItem>().HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ShopCartItem>().HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ShopCartItem>().HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ShopOrder>().Property(x => x.Status).IsConcurrencyToken();
            modelBuilder.Entity<StockBatch>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StockBatch>().HasIndex(x => new { x.ProductId, x.ExpiryDate });
            modelBuilder.Entity<StockBatch>().ToTable("StockBatches", t => t.HasCheckConstraint("CK_StockBatches_Quantity", "[Quantity] >= 0"));
            modelBuilder.Entity<OrderBatchAllocation>().HasOne(x => x.ShopOrderItem).WithMany().HasForeignKey(x => x.ShopOrderItemId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderBatchAllocation>().HasOne(x => x.StockBatch).WithMany().HasForeignKey(x => x.StockBatchId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StoreInfo>().ToTable("StoreInfos", t =>
                t.HasCheckConstraint("CK_StoreInfos_Single", "[Id] = 1"));
            modelBuilder.Entity<StoreInfo>().Property(s => s.Id).ValueGeneratedNever();

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");

                entity.HasIndex(c => c.Name)
                    .IsUnique();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products", table =>
                {
                    table.HasCheckConstraint(
                        "CK_Products_Price",
                        "[Price] > 0");

                    table.HasCheckConstraint(
                        "CK_Products_Stock",
                        "[Stock] >= 0");
                });

                entity.Property(p => p.Price)
                    .HasPrecision(18, 2);

                entity.Property(p => p.Stock)
                    .HasDefaultValue(0);

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
