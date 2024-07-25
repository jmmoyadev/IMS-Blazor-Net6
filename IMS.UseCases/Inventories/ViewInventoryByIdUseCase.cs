using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.Inventories
{
    public class ViewInventoryByIdUseCase : IViewInventoryByIdUseCase
    {
        private readonly IInventoryRepository _inventoryRepository;

        public ViewInventoryByIdUseCase(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<Inventory> ExecuteAsync(int id)
        {
            return await _inventoryRepository.GetInventoryByIdAsync(id);
        }
    }
}