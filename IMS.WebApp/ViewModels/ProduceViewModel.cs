using IMS.CoreBusiness;
using IMS.WebApp.ViewModelsValidations;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModels
{
    public class ProduceViewModel
    {
        [Required]
        public string ProductionNumber { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "You have to select a product")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity has to be greater than 0")]
        [Produce_EnsureEnoughInventoryQuantity]
        public int QuantityToProduce { get; set; }

        public decimal ProductPrice { get; set; }
    }
}