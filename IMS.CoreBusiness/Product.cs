using IMS.CoreBusiness.Validations;
using System.ComponentModel.DataAnnotations;

namespace IMS.CoreBusiness
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Quality must be greater or equal to 0")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Price must be greater or equal to 0")]
        public decimal Price { get; set; }

        [Product_EnturePricesIsGreaterThanInventoriesCost]
        public List<ProductInventory> ProductInventories { get; set; } = new();

        public void AddInventory(Inventory inventory)
        {
            if (!this.ProductInventories.Any(x => x.InventoryId == inventory.InventoryId))
            {
                this.ProductInventories.Add(new ProductInventory()
                {
                    InventoryId = inventory.InventoryId,
                    Inventory = inventory,
                    InventoryQuantity = 1,
                    ProductId = this.ProductId,
                    Product = this
                });
            }
        }
    }
}