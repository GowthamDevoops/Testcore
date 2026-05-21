using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    public class Registerviewmodel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType (DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType (DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        // [NotMapped]
        public string ConfirmPassword { get; set; }
    }
}
