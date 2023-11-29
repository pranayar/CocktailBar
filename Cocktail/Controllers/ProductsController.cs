using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Cocktail.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Cocktail.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IConfiguration Configuration;

        public ProductsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult FruitCocktails() 
        {
            List<Products> productsList = new List<Products>();
            
            productsList = fetch("Fruit");
            if (productsList != null)
            {
                return View("ProductView",productsList);
            }
            else
            {
                return View("Error");
            }


        }
        [HttpGet]
        public ActionResult BeerCocktails()
        {
            List<Products> productsList = new List<Products>();

            productsList = fetch("Beer");
            if (productsList != null)
            {
                return View("ProductView", productsList);
            }
            else
            {
                return View("Error");
            }


        }
        [HttpGet]
        public ActionResult VodkaCocktails()
        {
            List<Products> productsList = new List<Products>();

            productsList = fetch("Vodka");
            if (productsList != null)
            {
                return View("ProductView", productsList);
            }
            else
            {
                return View("Error");
            }


        }
        public List<Products> fetch(string category)
        {
            List<Products> productList = new List<Products>();
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    string query = "SELECT * FROM products where category =@Category or category='optional'";
                   
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Category", category);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Products product = new Products
                                    {
                                        Id = Convert.ToInt32(reader["Id"]),
                                        Name = Convert.ToString(reader["Name"]),
                                        Description = Convert.ToString(reader["Description"]),
                                        Amount = Convert.ToDouble(reader["Amount"]),
                                        Quantity = Convert.ToDouble(reader["Quantity"]),
                                        
                                        Category = Convert.ToString(reader["Category"])

                                    };

                                    productList.Add(product);
                                }
                            }
                        }
                    }
                }
                /*using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    string query = "SELECT * FROM options";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.CommandType = CommandType.Text;
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Products product = new Products
                                    {
                                        Id = Convert.ToInt32(reader["id"]),
                                        Name = Convert.ToString(reader["name"]),
                                        Amount = Convert.ToDouble(reader["amount"]),
                                        Category = "O"

                                    };

                                    productList.Add(product);
                                }
                            }
                        }
                    }
                }*/
            }
            catch (Exception)
            {
                return null;
            }
            return productList;
        }
        public JsonResult AddToCart(int productid,int quantity)
        {
            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId != null)
                {
                    string connectionString = this.Configuration.GetConnectionString("DefaultConnection");

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {

                        conn.Open();
                        string query = "SELECT * FROM cart where productid=@productid and userid=@userid";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {

                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@productid", productid);
                            cmd.Parameters.AddWithValue("@userid", userId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    int q = Convert.ToInt32(reader["quantityCart"]) + 1;
                                    string? cartid = Convert.ToString(reader["cartid"]);
                                    reader.Close();
                                    string query2 = "update cart set quantityCart=@quantity where cartid=@cartid";

                                    using (SqlCommand cmd2 = new SqlCommand(query2, conn))
                                    {
                                        cmd2.CommandType = CommandType.Text;
                                        cmd2.Parameters.AddWithValue("@quantity", q);
                                        cmd2.Parameters.AddWithValue("@cartid", cartid);
                                        cmd2.ExecuteNonQuery();

                                    }
                                }
                                else
                                {
                                    if (quantity > 0)
                                    {
                                        reader.Close();
                                        string query2 = "insert into cart values(@userid,@productid,@quantity)";

                                        using (SqlCommand cmd2 = new SqlCommand(query2, conn))
                                        {
                                            cmd2.CommandType = CommandType.Text;
                                            cmd2.Parameters.AddWithValue("@quantity", quantity);
                                            cmd2.Parameters.AddWithValue("@productId", productid);
                                            cmd2.Parameters.AddWithValue("@userid", userId);

                                            cmd2.ExecuteNonQuery();

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
            return Json(new { success = true, message = "Added to cart!" });
        }
        public ActionResult ViewCart()
        {
            List<Products> productsList = new List<Products>();

            try
            {
                string? userId = HttpContext.Session.GetString("UserId");
                if (userId != null)
                {
                    string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {

                        conn.Open();
                        string query = "SELECT * FROM cart inner join products on cart.ProductId=Products.Id where userid=@userid;";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {

                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@userid", userId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        Products product = new Products
                                        {
                                            Id = Convert.ToInt32(reader["Id"]),
                                            Name = Convert.ToString(reader["Name"]),
                                            Description = Convert.ToString(reader["Description"]),
                                            Amount = Convert.ToDouble(reader["Amount"]),
                                            Quantity = Convert.ToDouble(reader["quantityCart"]),
                                            
                                            Category = Convert.ToString(reader["Category"])

                                        };

                                        productsList.Add(product);
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login","Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
            return View("ViewCart", productsList);
        }

        public JsonResult BuyNow()
        {
            try
            {
                string connectionString = this.Configuration.GetConnectionString("DefaultConnection");

                string? userId = HttpContext.Session.GetString("UserId");
                if (userId != null)
                {

                    List<Products> products = CartAction(userId);
                    /*string productIds = "";
                    double amount = 0.0;*/
                    DateTime date=DateTime.Now;
                    string dateString = date.ToString("yyyyMMddHHmmss");

                    // Calculate the sum of digits
                    int digitSum = CalculateDigitSum(dateString);
                    
                    foreach (Products product in products)
                    
                    
                    {

                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            string query = "INSERT INTO orders (x,Userid,productid,orderdate,totalamount,deliverystatus) " +
                           "VALUES (@x,@userid, @productid, @orderdate, @total, @dstatus)";

                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@userid", userId);
                                cmd.Parameters.AddWithValue("@productid", product.Id);
                                cmd.Parameters.AddWithValue("@orderdate", date);
                                cmd.Parameters.AddWithValue("@total", product.Amount*product.Quantity);
                                cmd.Parameters.AddWithValue("@dstatus", 0);
                                cmd.Parameters.AddWithValue("@x",10000+digitSum);

                                // Execute the INSERT query
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
            return Json(new { success = true, message = "Order completed!" });

        }
        public List<Products> CartAction(string userid)
        {
            
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            List <Products> products = new List<Products>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ProductId,QuantityCart,Amount FROM cart inner join products on cart.ProductId=Products.Id where userid=@userid;";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@userid", userid);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Products product = new Products
                                    {
                                        Id = Convert.ToInt32(reader["ProductId"]),
                                        Amount = Convert.ToDouble(reader["Amount"]),
                                        Quantity = Convert.ToDouble(reader["quantityCart"]),

                                    };

                                    products.Add(product);
                                }
                            }

                        }
                    }
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    string query = "DELETE FROM cart WHERE userid = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", userid);
                        
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception) {
               //handle exception
            }
            return products;
        }

        public ActionResult ViewOrderHistory()
        {
            string? userId = HttpContext.Session.GetString("UserId");
            List<Order> orders=new List<Order>();
            if (userId != null)
            {
                
                //select * from orders inner join products on products.id=Orders.ProductId where userid='user12345';
                try
                {
                    string connectionString = this.Configuration.GetConnectionString("DefaultConnection");

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "select * from orders inner join products on products.id=Orders.ProductId where userid=@userid and DeliveryStatus=0";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            double amount = 0.0;

                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@userid", userId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        Order order = new Order
                                        {
                                            OrderId = Convert.ToInt32(reader["OrderId"]),
                                            product=new Products { Name = Convert.ToString(reader["name"]),
                                                Description = Convert.ToString(reader["Description"]),
                                                Amount = Convert.ToDouble(reader["Amount"]),
                                                Quantity = Convert.ToDouble(reader["Quantity"]),
                                                Category = Convert.ToString(reader["Category"])
                                            },
                                            OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                            OrderStatus = Convert.ToInt32(reader["deliverystatus"])
                                            
                                        };
                                        amount += order.product.Amount;
                                        
                                        orders.Add(order);
                                    }
                                    orders[0].TotalAmount = amount;
                                }

                            }
                        }
                    }
                }
                catch(Exception) { }
            }
            else
                return RedirectToAction("Login", "Home");

            return View("ViewOrderHistory", orders);
        }
         int CalculateDigitSum(string input)
        {
            int sum = 0;

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    sum += int.Parse(c.ToString());
                }
            }

            return sum;
        }
    }
}
