using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MarketWebApp.ViewModel
{
    public class EditCategoryViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Department Name")]
        [Required(ErrorMessage = "Department Name is required")]
        [MaxLength(50, ErrorMessage = "Department Name must be less than or equal to 50 characters.")]
        [MinLength(3, ErrorMessage = "Department Name must be at least 3 characters.")]
        [RegularExpression(@"^[a-zA-Z _-]+$", ErrorMessage = "Department name can only contain letters and spaces")]
      //  [Remote(action: "CheckCategoryExistEdit", controller: "Categories", ErrorMessage = "This Department Name Already Exists")]

        public string Name { get; set; }

        public string? Image { get; set; }
        [Display(Name = "Picture")]
     //   [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Invalid file format. Only JPG, JPEG, and PNG are allowed.")]

        public IFormFile? CategoryImage { get; set; }
    }

}
