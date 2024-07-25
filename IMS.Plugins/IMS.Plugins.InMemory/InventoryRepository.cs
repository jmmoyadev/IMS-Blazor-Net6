using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly List<Inventory> _inventories;

        public InventoryRepository()
        {
            _inventories = new List<Inventory>()
            {
                new Inventory() { InventoryId = 1, InventoryName = "Bike Seat", Quantity = 10, Price = 2 },
                new Inventory() { InventoryId = 2, InventoryName = "Bike Body", Quantity = 10, Price = 15 },
                new Inventory() { InventoryId = 3, InventoryName = "Bike Wheels", Quantity = 20, Price = 8 },
                new Inventory() { InventoryId = 4, InventoryName = "Bike Pedels", Quantity = 20, Price = 1 }
            };
        }

        public async Task<bool> ExistsInventoryAsync(Inventory inventory)
        {
            return await Task.FromResult(_inventories.Any(x => x.InventoryName.Equals(inventory.InventoryName, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await Task.FromResult(_inventories);
            }

            return _inventories.Where(p => p.InventoryName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Inventory> GetInventoryByIdAsync(int id)
        {
            var inv = _inventories.Single(p => p.InventoryId == id);

            var newInv = new Inventory()
            {
                InventoryId = inv.InventoryId,
                InventoryName = inv.InventoryName,
                Price = inv.Price,
                Quantity = inv.Quantity,
            };

            return await Task.FromResult(newInv);
        }

        public Task AddInventoryAsync(Inventory inventory)
        {
            var maxId = _inventories.Max(x => x.InventoryId);
            inventory.InventoryId = maxId + 1;

            _inventories.Add(inventory);

            return Task.CompletedTask;
        }

        public Task UpdateInventoryAsync(Inventory inventory)
        {
            if (_inventories.Any(x => x.InventoryId != inventory.InventoryId && x.InventoryName.Equals(inventory.InventoryName, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask;
            }

            var inv = _inventories.SingleOrDefault(x => x.InventoryId == inventory.InventoryId);
            if (inv != null)
            {
                inv.InventoryName = inventory.InventoryName;
                inv.Price = inventory.Price;
                inv.Quantity = inventory.Quantity;
            }

            return Task.CompletedTask;
        }
    }
}