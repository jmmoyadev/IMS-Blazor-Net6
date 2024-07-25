using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class ProductEFCoreRepository : IProductRepository
    {
        private readonly IDbContextFactory<IMSDbContext> _contextFactory;

        public ProductEFCoreRepository(IDbContextFactory<IMSDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> ExistsProductAsync(Product product)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Products.AnyAsync(p => p.ProductName.ToLower().Contains(product.ProductName.ToLower()));
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Products.Where(p => p.ProductName.ToLower().Contains(name.ToLower()))
                            .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Products
                                    .Include(p => p.ProductInventories)
                                        .ThenInclude(p => p.Inventory)
                                    .SingleAsync(p => p.ProductId == id);
        }

        public async Task AddProductAsync(Product product)
        {
            using var context = _contextFactory.CreateDbContext();

            context.Products.Add(product);

            FlagInventoryUnchanged(product, context);

            await context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            using var context = _contextFactory.CreateDbContext();

            var prod = await context.Products.Include(p => p.ProductInventories)
                                            .SingleAsync(p => p.ProductId == product.ProductId);
                                            
            if (prod != null)
            {
                prod.ProductName = product.ProductName;
                prod.Price = product.Price;
                prod.Quantity = product.Quantity;
                prod.ProductInventories = product.ProductInventories;

                FlagInventoryUnchanged(prod, context);

                await context.SaveChangesAsync();
            }
        }

        private void FlagInventoryUnchanged(Product product, IMSDbContext context)
        {
            if (product?.ProductInventories != null
             && product.ProductInventories.Count > 0)
            {
                foreach (var prodInv in product.ProductInventories)
                {
                    if (prodInv.Inventory != null)
                    {
                        context.Entry(prodInv.Inventory).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}