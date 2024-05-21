using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MarketWebApp.ViewModel.Supplier
{
    public class AddSupplierViewModel
    {
        [Display(Name = "Supplier Name")]
        [MaxLength(50, ErrorMessage = "Supplier Name Must Be Less Than 50 Char")]
        [MinLength(3, ErrorMessage = "Supplier Name Must be More Than 3 Char")]
        [Remote(action: "CheckSupplierExist", controller: "Suppliers", ErrorMessage = "This Supplier Name Already Exists")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Supplier Name must not contain numbers or special characters")]
        [Required(ErrorMessage = "Please Enter Supplier Name")]
        public string Name { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Please Enter Phone Number")]
        [RegularExpression(@"^(010|011|012|015)[0-9]{8}$", ErrorMessage = "Invalid Phone Number. Phone number should start with '010', '011', '012', or '015' and have 11 digits.")]
        public string Phone { get; set; }

        [Display(Name = "Location Name")]
        [Required(ErrorMessage = "Location Name is required")]
        [MaxLength(100, ErrorMessage = "Location Name must be less than or equal to 50 characters.")]
        [MinLength(3, ErrorMessage = "Location Name must be at least 3 characters.")]
        [RegularExpression(@"^[a-zA-Z]{3}[a-zA-Z0-9\s]*$", ErrorMessage = "Location name must contains at least 3 letters and can contain letters, numbers, and spaces")]
        public string Location { get; set; }
    }



    public class SupplierViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
        public string Location { get; set; }
    }
}
