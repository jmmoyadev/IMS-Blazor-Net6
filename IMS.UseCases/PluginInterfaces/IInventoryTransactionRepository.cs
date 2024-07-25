using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IInventoryTransactionRepository
    {
        Task PurchaseAsync(string poNumber, Inventory inventory, int quantity, string doneBy, decimal price);

        Task ProduceAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, decimal? price);

        Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionsAsync(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? transactionType);
    }
}