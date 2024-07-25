using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModels
{
    public class PurchaseViewModel
    {
        [Required]
        public string PONumber { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "You have to select an inventory")]
        public int InventoryId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity has to be greater than 0")]
        public int QuantityToPurchase { get; set; }

        public decimal InventoryPrice { get; set; }
    }
}