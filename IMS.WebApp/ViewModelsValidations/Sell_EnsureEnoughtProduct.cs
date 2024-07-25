using IMS.WebApp.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModelsValidations
{
    public class Sell_EnsureEnoughtProduct : ValidationAttribute
    {
        public Sell_EnsureEnoughtProduct()
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var sellViewModel = validationContext.ObjectInstance as SellViewModel;

            if (sellViewModel != null
             && sellViewModel.Product != null)
            {
                if (sellViewModel.QuantityToSell > sellViewModel.Product.Quantity)
                {
                    return new ValidationResult($"There isn't enought product. There is only {sellViewModel.Product.Quantity} in the warehouse",
                        new[] { validationContext.MemberName });
                }
            }

            return ValidationResult.Success;
        }
    }
}