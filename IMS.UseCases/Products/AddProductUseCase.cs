using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.Products
{
    public class AddProductUseCase : IAddProductUseCase
    {
        private readonly IProductRepository _productRepository;

        public AddProductUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task ExecuteAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (!(await _productRepository.ExistsProductAsync(product)))
                await this._productRepository.AddProductAsync(product);
        }
    }
}