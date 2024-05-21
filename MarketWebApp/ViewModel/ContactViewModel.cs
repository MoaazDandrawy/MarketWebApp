using System.ComponentModel.DataAnnotations;

namespace MarketWebApp.ViewModel
{
    public class ContactViewModel
    {

        public string Name { get; set; }


        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
