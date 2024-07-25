using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.InMemory
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly IInventoryRepository _inventoryRepository;

        public List<InventoryTransaction> _inventoryTransactions = new List<InventoryTransaction>();

        public InventoryTransactionRepository(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task PurchaseAsync(string poNumber, Inventory inventory, int quantity, string doneBy, decimal price)
        {
            this._inventoryTransactions.Add(new InventoryTransaction()
            {
                PurchaseOrderNumber = poNumber,

                Inventory = inventory,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                QuantityAfter = inventory.Quantity + quantity,
                UnitPrice = price,
                ActivityType = InventoryTransactionType.PurchaseInventory,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
            });

            await Task.CompletedTask;
        }

        public async Task ProduceAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, decimal? price)
        {
            this._inventoryTransactions.Add(new InventoryTransaction()
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

            await Task.CompletedTask;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionsAsync(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? transactionType)
        {
            var inventories = (await _inventoryRepository.GetInventoriesByNameAsync(string.Empty)).ToList();

            var query = this._inventoryTransactions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(inventoryName))
                query = query.Where(p => p.Inventory.InventoryName.Contains(inventoryName.ToLower()));

            if (dateFrom.HasValue)
                query = query.Where(p => p.TransactionDate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(p => p.TransactionDate <= dateTo.Value);

            if (transactionType.HasValue)
                query = query.Where(p => p.ActivityType == transactionType.Value);

            query = query.Join(inventories,
                                it => it.InventoryId,
                                inv => inv.InventoryId,
                                (it, inv) => new { it, inv })
                        .Select(p => new InventoryTransaction()
                        {
                            Inventory = p.inv,
                            Id = p.it.Id,
                            ProductionNumber = p.it.ProductionNumber,
                            PurchaseOrderNumber = p.it.PurchaseOrderNumber,
                            ActivityType = p.it.ActivityType,
                            InventoryId = p.it.InventoryId,
                            QuantityBefore = p.it.QuantityBefore,
                            QuantityAfter = p.it.QuantityAfter,
                            TransactionDate = p.it.TransactionDate,
                            DoneBy = p.it.DoneBy,
                            UnitPrice = p.it.UnitPrice,
                        });

            return query;
        }
    }
}