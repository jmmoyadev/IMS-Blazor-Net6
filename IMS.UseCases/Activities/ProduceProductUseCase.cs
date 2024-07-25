using IMS.CoreBusiness;

using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Activities
{
    public class ProduceProductUseCase : IProduceProductUseCase
    {
        private readonly IProductTransactionRepository _productTransactionRepository;
        private readonly IProductRepository _productRepository;

        public ProduceProductUseCase(
            IProductTransactionRepository productTransactionRepository,
            IProductRepository productRepository)
        {
            _productTransactionRepository = productTransactionRepository;
            _productRepository = productRepository;
        }

        public async Task ExecuteAsync(string productionNumber, Product product, int quantity, string doneBy)
        {
            // add transaction record

            this._productTransactionRepository.ProduceAsync(productionNumber, product, quantity, doneBy);

            // decrease the quantity inventories

            // update the quantity of product
            product.Quantity += quantity;
            await this._productRepository.UpdateProductAsync(product);
        }
    }
}