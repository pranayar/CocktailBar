namespace Cocktail.Models
{
    public class Order
    {
        public int OrderId { get; set; }
         
        public Products? product { get; set; }

        public Double TotalAmount { get; set;}

        public DateTime? OrderDate { get; set; }

        public int OrderStatus { get; set; }
        public Customers? customer { get; set; }


    }
}
