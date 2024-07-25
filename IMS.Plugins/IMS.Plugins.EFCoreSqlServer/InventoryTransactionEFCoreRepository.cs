using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class InventoryTransactionEFCoreRepository : IInventoryTransactionRepository
    {
        private readonly IDbContextFactory<IMSDbContext> _contextFactory;

        public InventoryTransactionEFCoreRepository(IDbContextFactory<IMSDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task PurchaseAsync(string poNumber, Inventory inventory, int quantity, string doneBy, decimal price)
        {
            using var context = _contextFactory.CreateDbContext();

            context.InventoryTransactions.Add(new InventoryTransaction()
            {
                PurchaseOrderNumber = poNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                QuantityAfter = inventory.Quantity + quantity,
                UnitPrice = price,
                ActivityType = InventoryTransactionType.PurchaseInventory,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
            });

            await context.SaveChangesAsync();
        }

        public async Task ProduceAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, decimal? price)
        {
            using var context = _contextFactory.CreateDbContext();

            context.InventoryTransactions.Add(new InventoryTransaction()
            {
                ProductionNumber = productionNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                QuantityAfter = inventory.Quantity - quantityToConsume,
                UnitPrice = price,
                TransactionDate = DateTime.UtcNow,
                ActivityType = InventoryTransactionType.ProduceProduct,
                DoneBy = doneBy,
            });

            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionsAsync(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? transactionType)
        {
            using var context = _contextFactory.CreateDbContext();

            var query = context.InventoryTransactions
                                        .Include(p => p.Inventory)
                                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(inventoryName))
                query = query.Where(p => p.Inventory.InventoryName.Contains(inventoryName.ToLower()));

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