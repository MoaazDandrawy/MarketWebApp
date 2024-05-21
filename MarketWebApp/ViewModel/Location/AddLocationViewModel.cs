using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MarketWebApp.ViewModel.Location
{
    public class AddLocationViewModel
    {
        [Display(Name = "Location Name")]
        [Required(ErrorMessage = "Location Name is required")]
        [MaxLength(100, ErrorMessage = "Location Name must be less than or equal to 50 characters.")]
        [MinLength(3, ErrorMessage = "Location Name must be at least 3 characters.")]
        [RegularExpression(@"^[a-zA-Z]{3}[a-zA-Z0-9\s]*$", ErrorMessage = "Location name must contains at least 3 letters and can contain letters, numbers, and spaces")]
        [Remote(action: "CheckLocationExist", controller: "Locations", ErrorMessage = "This Location is Already Exists")]

        public string Name { get; set; }

    }
}
