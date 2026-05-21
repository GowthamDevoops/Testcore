using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TestApp.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
      //  [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Quantity { get; set; }
       // public int UnitPrice { get; set; }
        public string userid { get; set; }
         
    }
}
