using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Activities
{
    public class PurchaseInventoryUseCase : IPurchaseInventoryUseCase
    {
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public PurchaseInventoryUseCase(
            IInventoryTransactionRepository inventoryTransactionRepository,
            IInventoryRepository inventoryRepository)
        {
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task ExecuteAsync(string poNumber, Inventory inventory, int quantity, string doneBy)
        {
            //insert a record in the transaction table
            await _inventoryTransactionRepository.PurchaseAsync(poNumber, inventory, quantity, doneBy, inventory.Price);

            // increase the quantity
            inventory.Quantity += quantity;

            await this._inventoryRepository.UpdateInventoryAsync(inventory);
        }
    }
}