using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModels
{
    public class ManageUserViewModel
    {
        public string? Email { get; set; }

        [Required]
        public string? Department { get; set; }
    }
}