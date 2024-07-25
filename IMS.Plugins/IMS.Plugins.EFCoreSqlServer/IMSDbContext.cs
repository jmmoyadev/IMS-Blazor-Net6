using IMS.CoreBusiness;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class IMSDbContext : DbContext
    {
        public IMSDbContext(DbContextOptions<IMSDbContext> options) : base(options)
        {
        }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductInventory> ProductsInventories { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<ProductTransaction> ProductTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(p => p.ProductId);

            modelBuilder.Entity<Inventory>().HasKey(p => p.InventoryId);

            modelBuilder.Entity<ProductInventory>()
                        .HasKey(p => new { p.ProductId, p.InventoryId });

            modelBuilder.Entity<ProductInventory>()
                        .HasOne(p => p.Product)
                        .WithMany(p => p.ProductInventories)
                        .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<ProductInventory>()
                        .HasOne(p => p.Inventory)
                        .WithMany(p => p.ProductInventories)
                        .HasForeignKey(p => p.InventoryId);

            #region "Seeding data"

            modelBuilder.Entity<Inventory>().HasData(
                new Inventory() { InventoryId = 1, InventoryName = "Bike Seat", Quantity = 10, Price = 2 },
                new Inventory() { InventoryId = 2, InventoryName = "Bike Body", Quantity = 10, Price = 15 },
                new Inventory() { InventoryId = 3, InventoryName = "Bike Wheels", Quantity = 20, Price = 8 },
                new Inventory() { InventoryId = 4, InventoryName = "Bike Pedels", Quantity = 20, Price = 1 }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product() { ProductId = 1, ProductName = "Bike", Quantity = 10, Price = 150 },
                new Product() { ProductId = 2, ProductName = "Car", Quantity = 5, Price = 25000 }
                );

            modelBuilder.Entity<ProductInventory>().HasData(
                new ProductInventory() { ProductId = 1, InventoryId = 1, InventoryQuantity = 1 },
                new ProductInventory() { ProductId = 1, InventoryId = 2, InventoryQuantity = 1 },
                new ProductInventory() { ProductId = 1, InventoryId = 3, InventoryQuantity = 2 },
                new ProductInventory() { ProductId = 1, InventoryId = 4, InventoryQuantity = 2 }
               );

            #endregion "Seeding data"
        }
    }
}