using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MarketWebApp.ViewModel
{
    public class AddCategoryViewModel
    {
        [Display(Name = "Department Name")]
        [Required(ErrorMessage = "Department Name is required")]
        [MaxLength(50, ErrorMessage = "Department Name must be less than or equal to 50 characters.")]
        [MinLength(3, ErrorMessage = "Department Name must be at least 3 characters.")]
        [RegularExpression(@"^[a-zA-Z _-]+$", ErrorMessage = "Department name can only contain letters and spaces")]
        [Remote(action: "CheckCategoryExist", controller: "Categories", ErrorMessage = "This Department Name Already Exists")]

        public string? Name { get; set; }

        [Required(ErrorMessage = "Please choose Department Picture ")]
        [Display(Name = "Department Picture")]
        [AllowedExtensions([".jpg", ".jpeg", ".png"], ErrorMessage = "Invalid file format. Only JPG, JPEG, and PNG are allowed.")]
        public IFormFile? CategoryImage { get; set; }
    }

    public class CategoryViewModel
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public IFormFile? CategoryImage { get; set; }

    }

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        public override bool IsValid(object value)
        {
            if (value is IFormFile file)
            {
                var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
                return _extensions.Contains(extension);
            }
            return false;
        }

    }
}
