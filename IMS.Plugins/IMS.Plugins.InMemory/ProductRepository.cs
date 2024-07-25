using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class ProductRepository : IProductRepository
    {
        private readonly List<Product> _products;

        public ProductRepository()
        {
            _products = new List<Product>()
            {
                new Product() { ProductId = 1, ProductName = "Bike", Quantity = 10, Price = 150 },
                new Product() { ProductId = 2, ProductName = "Car", Quantity = 5, Price = 25000 },
            };
        }

        public async Task<bool> ExistsProductAsync(Product product)
        {
            return await Task.FromResult(_products.Any(x => x.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await Task.FromResult(_products);
            }

            return _products.Where(p => p.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = _products.Single(p => p.ProductId == id);

            var newProduct = new Product()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = product.Quantity,
            };

            if (product.ProductInventories.Any())
            {
                foreach (var prodInv in product.ProductInventories)
                {
                    var newProdInv = new ProductInventory()
                    {
                        InventoryId = prodInv.InventoryId,
                        ProductId = prodInv.ProductId,
                        Product = newProduct,
                        Inventory = new Inventory(),
                        InventoryQuantity = prodInv.InventoryQuantity
                    };
                    if (prodInv.Inventory != null)
                    {
                        newProdInv.Inventory = new Inventory()
                        {
                            InventoryId = prodInv.InventoryId,
                            InventoryName = prodInv.Inventory.InventoryName,
                            Price = prodInv.Inventory.Price,
                            Quantity = prodInv.Inventory.Quantity
                        };

                    }

                    newProduct.ProductInventories.Add(newProdInv);
                }
            }


            return await Task.FromResult(newProduct);
        }

        public Task AddProductAsync(Product product)
        {
            var maxId = _products.Max(x => x.ProductId);
            product.ProductId = maxId + 1;

            _products.Add(product);

            return Task.CompletedTask;
        }

        public Task UpdateProductAsync(Product product)
        {
            if (_products.Any(x => x.ProductId != product.ProductId && x.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask;
            }

            var prod = _products.SingleOrDefault(x => x.ProductId == product.ProductId);
            if (prod != null)
            {
                prod.ProductName = product.ProductName;
                prod.Price = product.Price;
                prod.Quantity = product.Quantity;
                prod.ProductInventories = product.ProductInventories;
            }


            return Task.CompletedTask;
        }
    }
}