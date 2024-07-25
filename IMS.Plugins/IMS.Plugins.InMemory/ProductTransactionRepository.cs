using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using System.Runtime.CompilerServices;

namespace IMS.Plugins.InMemory
{
    public class ProductTransactionRepository : IProductTransactionRepository
    {
        private List<ProductTransaction> _productTransactions = new();

        private readonly IProductRepository _productRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;

        public ProductTransactionRepository(
            IProductRepository productRepository,
            IInventoryRepository inventoryRepository,
            IInventoryTransactionRepository inventoryTransactionRepository)
        {
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
        }

        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy)
        {
            // decrase the inventories
            var prod = _productRepository.GetProductByIdAsync(product.ProductId);

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
                                          doneBy,null);

                        // decrease the inventories
                        var inv = await _inventoryRepository.GetInventoryByIdAsync(pi.InventoryId);
                        inv.Quantity -= pi.InventoryQuantity * quantity;
                        await _inventoryRepository.UpdateInventoryAsync(inv);
                    }
                }
            }
            // add inventory transaction

            _productTransactions.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                QuantityAfter = product.Quantity + quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy
            });

            // add product transaction
        }

        public async Task SellProductAsync(string salesOrderNumber, Product product, int quantity, decimal unitPrice, string doneBy)
        {
            this._productTransactions.Add(new ProductTransaction()
            {
                SalesOrderNumber = salesOrderNumber,
                Product = product,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                QuantityAfter = product.Quantity - quantity,
                UnitPrice = unitPrice,
                ActivityType = ProductTransactionType.SellProduct,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
            });

            await Task.CompletedTask;
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionsAsync(string productName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? transactionType)
        {
            var products = (await _productRepository.GetProductsByNameAsync(string.Empty)).ToList();

            var query = this._productTransactions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(productName))
                query = query.Where(p => p.Product.ProductName.Contains(productName.ToLower()));

            if (dateFrom.HasValue)
                query = query.Where(p => p.TransactionDate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(p => p.TransactionDate <= dateTo.Value);

            if (transactionType.HasValue)
                query = query.Where(p => p.ActivityType == transactionType.Value);

            query = query.Join(products,
                                pt => pt.ProductId,
                                prod => prod.ProductId,
                                (pt, prod) => new { pt, prod })
                        .Select(p => new ProductTransaction()
                        {
                            Product = p.prod,
                            Id = p.pt.Id,
                            SalesOrderNumber = p.pt.SalesOrderNumber,
                            ProductionNumber = p.pt.ProductionNumber,
                            ActivityType = p.pt.ActivityType,
                            ProductId = p.pt.ProductId,
                            QuantityBefore = p.pt.QuantityBefore,
                            QuantityAfter = p.pt.QuantityAfter,
                            TransactionDate = p.pt.TransactionDate,
                            DoneBy = p.pt.DoneBy,
                            UnitPrice = p.pt.UnitPrice,
                        });

            return query;
        }
    }
}