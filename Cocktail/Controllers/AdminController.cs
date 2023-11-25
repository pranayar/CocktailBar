using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Cocktail.Models;
using static NuGet.Packaging.PackagingConstants;

namespace Cocktail.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration Configuration;

        public AdminController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }
        public Boolean IsAdmin ()
        {
            if( HttpContext.Session.GetInt32("adminid")>0) { return true; }
            else { return false; }
        }
        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            try
            {
                string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection conn = new SqlConnection(connectionString))

                {
                    conn.Open();
                    string query = "SELECT AdminId,FirstName + ' ' + LastName AS name FROM admin WHERE (username = @username) AND password = @password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                int adminid = reader.GetInt32("adminid");
                                string userName = reader.GetString("name");

                                // Store the user ID and name in the session
                                HttpContext.Session.SetInt32("adminid", adminid);
                                HttpContext.Session.SetString("name", userName);

                                if (adminid >0)
                                {
                                    return RedirectToAction("fetchAllProducts");
                                    
                                }
                            }
                            else
                            {
                                // Handle invalid login credentials here, e.g., display an error message to the user
                                TempData["ErrorMessage"] = "Invalid username or password.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View("Error");
            }

            return View();
        }
        public ActionResult fetchAllProducts()
        {
            if(!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            List<Products> productList = new List<Products>();
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            try
            {
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    string query = "SELECT * FROM products";

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
                                        Id = Convert.ToInt32(reader["Id"]),
                                        Name = Convert.ToString(reader["Name"]),
                                        Description = Convert.ToString(reader["Description"]),
                                        Amount = Convert.ToDouble(reader["Amount"]),
                                        Quantity = Convert.ToDouble(reader["Quantity"]),
                                        ImageLink = Convert.ToString(reader["ImageLink"]),
                                        Category = Convert.ToString(reader["Category"])

                                    };

                                    productList.Add(product);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Home","Error");
            }
            return View("ViewAllProducts", productList);
        }
        public ActionResult fetchAllCustomers()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            List<Customers> customerList = new List<Customers>();
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            try
            {
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    string query = "SELECT * FROM customers";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.CommandType = CommandType.Text;


                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Customers customer= new Customers
                                    {
                                        
                                        UserId = Convert.ToString(reader["userid"]),
                                        FirstName = Convert.ToString(reader["FirstName"]),
                                        LastName = Convert.ToString(reader["LastName"]),
                                        DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                        Email = Convert.ToString(reader["email"]),
                                        Contact = Convert.ToString(reader["contact"]),
                                        FullAddress = Convert.ToString(reader["FullAddress"])


                                    };

                                    customerList.Add(customer);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
            return View("ViewAllCustomers",customerList);
        }
        public ActionResult fetchAllOrders()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            List<Order> orders = new List<Order>();
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    string query = "select * from orders inner join products on products.id=Orders.ProductId inner join customers on customers.UserId=Orders.UserId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        double amount = 0.0;

                        cmd.CommandType = CommandType.Text;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Order order = new Order
                                    {
                                        OrderId = Convert.ToInt32(reader["OrderId"]),
                                        product = new Products
                                        {
                                            Name = Convert.ToString(reader["name"]),
                                            Description = Convert.ToString(reader["Description"]),
                                            Amount = Convert.ToDouble(reader["Amount"]),
                                            Quantity = Convert.ToDouble(reader["Quantity"]),
                                            Category = Convert.ToString(reader["Category"])
                                        },
                                        customer=new Customers
                                        {
                                            UserId = Convert.ToString(reader["userid"]),
                                            FirstName = Convert.ToString(reader["FirstName"]),
                                            LastName = Convert.ToString(reader["LastName"]),
                                            Email = Convert.ToString(reader["email"]),
                                            Contact = Convert.ToString(reader["contact"]),
                                            FullAddress = Convert.ToString(reader["FullAddress"])
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
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
            return View("fetchAllOrders",orders);
        }
        public ActionResult AddProduct()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            return View("Product/Add");
        }
        [HttpPost]
        public ActionResult AddProduct(Products product)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            if (ModelState.IsValid)
            {
                string connectionString = this.Configuration.GetConnectionString("DefaultConnection");

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = "INSERT INTO products (Name, Description, Amount, Quantity, Category) " +
                       "VALUES (@Name, @Description, @Amount, @Quantity, @Category)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Name", product.Name);
                            cmd.Parameters.AddWithValue("@Description", product.Description);
                            cmd.Parameters.AddWithValue("@Amount", product.Amount);
                            cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
                            cmd.Parameters.AddWithValue("@Category", product.Category);

                            // Execute the INSERT query
                            cmd.ExecuteNonQuery();
                        }
                    }


                }
                catch (Exception)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
        
            return RedirectToAction("fetchAllProducts");
        }
        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            Products product = new Products();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM products WHERE id = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", id);

                        // Execute the SELECT query to get information about the product
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Check if there is a row
                            if (reader.Read())
                            {
                                product = new Products
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = Convert.ToString(reader["Name"]),
                                    Description = Convert.ToString(reader["Description"]),
                                    Amount = Convert.ToDouble(reader["Amount"]),
                                    Quantity = Convert.ToDouble(reader["Quantity"]),
                                    ImageLink = Convert.ToString(reader["ImageLink"]),
                                    Category = Convert.ToString(reader["Category"])

                                };

                            }
                            else
                                return View("Error");
                        }
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }

            
            return View("Product/Delete", product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDeleteProduct(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    string query = "DELETE FROM products WHERE id = @id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@id", id);

                            // Execute the DELETE query
                            cmd.ExecuteNonQuery();
                        }
                }
            }
            
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("fetchAllProducts");
        }
        [HttpGet]
        public ActionResult EditProduct(int id) {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            Products product=new Products();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM products WHERE id = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", id);

                        // Execute the SELECT query to get information about the product
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Check if there is a row
                            if (reader.Read())
                            {
                                product = new Products
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = Convert.ToString(reader["Name"]),
                                    Description = Convert.ToString(reader["Description"]),
                                    Amount = Convert.ToDouble(reader["Amount"]),
                                    Quantity = Convert.ToDouble(reader["Quantity"]),
                                    ImageLink = Convert.ToString(reader["ImageLink"]),
                                    Category = Convert.ToString(reader["Category"])

                                };

                            }
                            else
                                return View("Error");
                        }
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }

            // If the product with the given ID is not found, you might handle it appropriately
            return View("Product/Edit",product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Products updatedProduct)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            if (ModelState.IsValid)
            {
                string connectionString = this.Configuration.GetConnectionString("DefaultConnection");

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = "UPDATE products " +
                                       "SET Name = @Name, " +
                                       "Description = @Description, " +
                                       "Amount = @Amount, " +
                                       "Quantity = @Quantity, " +
                                       "Category = @Category " +
                                       "WHERE Id = @Id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", updatedProduct.Id);
                            cmd.Parameters.AddWithValue("@Name", updatedProduct.Name);
                            cmd.Parameters.AddWithValue("@Description", updatedProduct.Description);
                            cmd.Parameters.AddWithValue("@Amount", updatedProduct.Amount);
                            cmd.Parameters.AddWithValue("@Quantity", updatedProduct.Quantity);
                           
                            cmd.Parameters.AddWithValue("@Category", updatedProduct.Category);

                            // Execute the UPDATE query
                            cmd.ExecuteNonQuery();
                        }
                    }

                    
                }
                catch (Exception)
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            // If ModelState is not valid, return to the Edit view with the updated product model
            return RedirectToAction("fetchAllProducts");
        }
        [HttpGet]
        public ActionResult DetailsProduct(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            Products product = new Products();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM products WHERE id = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", id);

                        // Execute the SELECT query to get information about the product
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Check if there is a row
                            if (reader.Read())
                            {
                                product = new Products
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = Convert.ToString(reader["Name"]),
                                    Description = Convert.ToString(reader["Description"]),
                                    Amount = Convert.ToDouble(reader["Amount"]),
                                    Quantity = Convert.ToDouble(reader["Quantity"]),
                                    ImageLink = Convert.ToString(reader["ImageLink"]),
                                    Category = Convert.ToString(reader["Category"])

                                };

                            }
                            else
                                return View("Error");
                        }
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }

            // If the product with the given ID is not found, you might handle it appropriately
            return View("Product/Details",product);
        }

        //Admin - Customer control
        public ActionResult DeleteCustomer(string id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            Customers customer=new Customers();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM customers WHERE userid = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", id);

                        // Execute the SELECT query to get information about the product
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Check if there is a row
                            if (reader.Read())
                            {
                                 customer = new Customers
                                {

                                    UserId = Convert.ToString(reader["userid"]),
                                    FirstName = Convert.ToString(reader["FirstName"]),
                                    LastName = Convert.ToString(reader["LastName"]),
                                    DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                    Email = Convert.ToString(reader["email"]),
                                    Contact = Convert.ToString(reader["contact"]),
                                    FullAddress = Convert.ToString(reader["FullAddress"])


                                };

                            }
                            else
                                return View("Error");
                        }
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }


            return View("Customer/Delete", customer);
        }
        [HttpPost]
        public ActionResult ConfirmDeleteCustomer(string id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    string query = "DELETE FROM customers WHERE userid = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", id);

                        // Execute the DELETE query
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("fetchAllCustomers");
        }
        public ActionResult AddCustomer()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            return View("Customer/Add");
        }
        [HttpPost]
        public ActionResult AddCustomer(Customers customer)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();


                        using (var cmd2 = conn.CreateCommand())
                        {
                            cmd2.CommandText = "INSERT INTO customers (email, contact, password,firstname,lastname,userid,FullAddress,DateOfBirth) VALUES (@Email, @Contact, @Password, @FirstName,@LastName,@UserID,@FullAddress,@Dateofbirth)";
                            cmd2.Parameters.AddWithValue("@Password", customer.Password);
                            cmd2.Parameters.AddWithValue("@Email", customer.Email);
                            cmd2.Parameters.AddWithValue("@Contact", customer.Contact);
                            cmd2.Parameters.AddWithValue("@UserID", customer.UserId);

                            cmd2.Parameters.AddWithValue("@FirstName", customer.FirstName);
                            cmd2.Parameters.AddWithValue("@LastName", customer.LastName);
                            cmd2.Parameters.AddWithValue("@FullAddress", customer.FullAddress);
                            cmd2.Parameters.AddWithValue("@Dateofbirth", customer.DateOfBirth);

                        cmd2.ExecuteNonQuery();
                    }
                    }


                }
                catch (Exception)
                {
                    return RedirectToAction("Error", "Home");
                }
            

            return RedirectToAction("fetchAllCustomers");
        }

        public ActionResult DetailsCustomer(string id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            Customers customer=new Customers();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM customers WHERE userid = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", id);

                        // Execute the SELECT query to get information about the product
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Check if there is a row
                            if (reader.Read())
                            {
                                 customer = new Customers
                                {

                                    UserId = Convert.ToString(reader["userid"]),
                                    FirstName = Convert.ToString(reader["FirstName"]),
                                    LastName = Convert.ToString(reader["LastName"]),
                                    DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                    Email = Convert.ToString(reader["email"]),
                                    Contact = Convert.ToString(reader["contact"]),
                                    FullAddress = Convert.ToString(reader["FullAddress"])


                                };

                            }
                            else
                                return View("Error");
                        }
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }

            
            return View("Customer/Details", customer);
        }

        public ActionResult EditCustomer(string id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
            Customers customer = new Customers();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM customers WHERE userid = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", id);

                        // Execute the SELECT query to get information about the product
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Check if there is a row
                            if (reader.Read())
                            {
                                customer = new Customers
                                {

                                    UserId = Convert.ToString(reader["userid"]),
                                    FirstName = Convert.ToString(reader["FirstName"]),
                                    LastName = Convert.ToString(reader["LastName"]),
                                    DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                    Email = Convert.ToString(reader["email"]),
                                    Contact = Convert.ToString(reader["contact"]),
                                    FullAddress = Convert.ToString(reader["FullAddress"])


                                };

                            }
                            else
                                return View("Error");
                        }
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
            return View("Customer/Edit", customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomer(Customers customer)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }
            string connectionString = this.Configuration.GetConnectionString("DefaultConnection");

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = "UPDATE customers " +
                                       "SET FirstName = @fname, " +
                                       "LastName = @lname," +
                                       "DateofBirth = @dob," +
                                       "Email = @Email," +
                                       "Contact = @Contact," +
                                       "FullAddress =@address,"+
                                       "Password=@password "+
                                      
                                       "WHERE UserId = @userid";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@fname", customer.FirstName);
                        cmd.Parameters.AddWithValue("@lname", customer.LastName);
                        cmd.Parameters.AddWithValue("@dob", customer.DateOfBirth);
                        cmd.Parameters.AddWithValue("@Email", customer.Email);
                        cmd.Parameters.AddWithValue("@Contact", customer.Contact);
                        cmd.Parameters.AddWithValue("@address", customer.FullAddress);
                        cmd.Parameters.AddWithValue("@password", customer.Password);
                        cmd.Parameters.AddWithValue("@userid", customer.UserId);
                        

                        // Execute the UPDATE query
                        cmd.ExecuteNonQuery();
                        }
                    }


                }
                catch (Exception)
                {
                    return RedirectToAction("Error", "Home");
                }
            

            // If ModelState is not valid, return to the Edit view with the updated product model
            return RedirectToAction("fetchAllCustomers");
        }


    }
}
