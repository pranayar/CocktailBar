using System.ComponentModel.DataAnnotations;

namespace Cocktail.Models
{
    public class Products
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Product name is required.")]
        public string? Name { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Product amount is required.")]
        public double Amount {  get; set; }
        public double Quantity {  get; set; }
       
        public string? Category { get; set; }
    }
}
