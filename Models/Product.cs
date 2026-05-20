using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [StringLength (500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }
        [Range(1, 200000, ErrorMessage = "Price must be between 1 and 200000.")]
        public decimal Price { get; set; }

        public string? Imagepath { get; set; }

        public DateTime CreatedAt { get; set; }
        [NotMapped]

        public IFormFile? ImageFile { get; set; }
    }
}
