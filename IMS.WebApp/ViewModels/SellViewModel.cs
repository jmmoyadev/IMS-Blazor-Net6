using IMS.CoreBusiness;
using IMS.WebApp.ViewModelsValidations;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModels
{
    public class SellViewModel
    {
        public string SalesOrderNumber { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "You have to select a product")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity has to be greater than 0")]
        [Sell_EnsureEnoughtProduct]
        public int QuantityToSell { get; set; }

        public decimal UnitPrice { get; set; }
    }
}