using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Products
{
    public class ViewProductsByNameUseCase : IViewProductsByNameUseCase
    {
        private readonly IProductRepository _producRepository;

        public ViewProductsByNameUseCase(IProductRepository producRepository)
        {
            _producRepository = producRepository;
        }

        public async Task<IEnumerable<Product>> ExecuteAsync(string name = "")
        {
            return await _producRepository.GetProductsByNameAsync(name);
        }
    }
}