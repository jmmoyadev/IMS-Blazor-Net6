using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsByNameAsync(string name);

        Task<bool> ExistsProductAsync(Product product);

        Task<Product> GetProductByIdAsync(int id);

        Task AddProductAsync(Product product);

        Task UpdateProductAsync(Product product);
    }
}