using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class InventoryEFCoreRepository : IInventoryRepository
    {
        private readonly IDbContextFactory<IMSDbContext> _contextFactory;

        public InventoryEFCoreRepository(IDbContextFactory<IMSDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> ExistsInventoryAsync(Inventory inventory)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Inventories.AnyAsync(p => p.InventoryName.ToLower().Contains(inventory.InventoryName.ToLower()));
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesByNameAsync(string name)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Inventories.Where(p => p.InventoryName.ToLower().Contains(name.ToLower())).ToListAsync();
        }

        public async Task<Inventory> GetInventoryByIdAsync(int inventoryId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Inventories.SingleAsync(p => p.InventoryId == inventoryId);
        }

        public async Task AddInventoryAsync(Inventory inventory)
        {
            using var context = _contextFactory.CreateDbContext();

            context.Inventories.Add(inventory);

            await context.SaveChangesAsync();
        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            using var context = _contextFactory.CreateDbContext();

            var inv = await context.Inventories.SingleAsync(p => p.InventoryId == inventory.InventoryId);

            if (inv != null)
            {
                inv.InventoryName = inventory.InventoryName;
                inv.Price = inventory.Price;
                inv.Quantity = inventory.Quantity;

                await context.SaveChangesAsync();
            }
        }
    }
}