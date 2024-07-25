using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class ProductTransactionEFCoreRepository : IProductTransactionRepository
    {
        private readonly IProductRepository _productRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
        private readonly IDbContextFactory<IMSDbContext> _contextFactory;

        public ProductTransactionEFCoreRepository(
            IProductRepository productRepository,
            IInventoryRepository inventoryRepository,
            IInventoryTransactionRepository inventoryTransactionRepository,
            IDbContextFactory<IMSDbContext> contextFactory)
        {
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _contextFactory = contextFactory;
        }

        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy)
        {
            using var context = _contextFactory.CreateDbContext();

            var prod = await _productRepository.GetProductByIdAsync(product.ProductId);
            if (prod != null)
            {
                foreach (var pi in product.ProductInventories)
                {
                    if (pi.Inventory != null)
                    {
                        // add inventory transaction
                        await _inventoryTransactionRepository
                            .ProduceAsync(productionNumber,
                                          pi.Inventory,
                                          pi.InventoryQuantity * quantity,
                                          doneBy,
                                          null);

                        // decrease the inventories
                        var inv = await _inventoryRepository.GetInventoryByIdAsync(pi.InventoryId);
                        inv.Quantity -= pi.InventoryQuantity * quantity;
                        await _inventoryRepository.UpdateInventoryAsync(inv);
                    }
                }
            }

            // add product transaction
            context.ProductTransactions.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                QuantityAfter = product.Quantity + quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy
            });

            await context.SaveChangesAsync();
        }

        public async Task SellProductAsync(string salesOrderNumber, Product product, int quantity, decimal unitPrice, string doneBy)
        {
            using var context = _contextFactory.CreateDbContext();

            // add product transaction
            context.ProductTransactions.Add(new ProductTransaction()
            {
                SalesOrderNumber = salesOrderNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                QuantityAfter = product.Quantity - quantity,
                UnitPrice = unitPrice,
                ActivityType = ProductTransactionType.SellProduct,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
            });


            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionsAsync(string productName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? transactionType)
        {
            using var context = _contextFactory.CreateDbContext();

            var query = context.ProductTransactions.Include(p => p.Product).AsQueryable();

            if (!string.IsNullOrWhiteSpace(productName))
                query = query.Where(p => p.Product != null && p.Product.ProductName.Contains(productName.ToLower()));

            if (dateFrom.HasValue)
                query = query.Where(p => p.TransactionDate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(p => p.TransactionDate <= dateTo.Value);

            if (transactionType.HasValue)
                query = query.Where(p => p.ActivityType == transactionType.Value);

            return await query.ToListAsync();
        }

 
    }
}