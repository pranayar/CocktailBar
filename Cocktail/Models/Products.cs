namespace Cocktail.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Amount {  get; set; }
        public double Quantity {  get; set; }
       
        public string? ImageLink {  get; set; }
        public string? Category { get; set; }
    }
}
