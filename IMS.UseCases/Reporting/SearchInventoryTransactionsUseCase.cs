using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Reporting
{
    public class SearchInventoryTransactionsUseCase : ISearchInventoryTransactionsUseCase
    {
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;

        public SearchInventoryTransactionsUseCase(IInventoryTransactionRepository inventoryTransactionRepository)
        {
            _inventoryTransactionRepository = inventoryTransactionRepository;
        }

        public async Task<IEnumerable<InventoryTransaction>> ExecuteAsync(
            string inventoryName,
            DateTime? dateFrom,
            DateTime? dateTo,
            InventoryTransactionType? transactionType)

        {
            if (dateTo.HasValue)
                dateTo = dateTo.Value.Date;

            if (dateTo.HasValue)
                dateTo = dateTo.Value.Date.AddDays(1);

            return await this._inventoryTransactionRepository.GetInventoryTransactionsAsync(
                inventoryName,
                dateFrom,
                dateTo,
                transactionType);
        }
    }
}