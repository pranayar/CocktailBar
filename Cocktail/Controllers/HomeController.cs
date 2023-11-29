using Cocktail.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Cocktail.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration Configuration;

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        public ActionResult About()
        {
            return View("About");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("FruitCocktails","Products");
            }
            return View();
        }
        [HttpPost]

        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {

            try
            {
                string connectionString = this.Configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection conn = new SqlConnection(connectionString))

                {
                    conn.Open();
                    string query = "SELECT userid,FirstName + ' ' + LastName AS name FROM customers WHERE (userid = @userid) AND password = @password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@userid", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                string userId = reader.GetString("userid");
                                string userName = reader.GetString("name");

                                // Store the user ID and name in the session
                                HttpContext.Session.SetString("UserId", userId);
                                HttpContext.Session.SetString("name", userName);

                                if (userId != null)
                                {
                                    return RedirectToAction("FruitCocktails", "Products");
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

        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(Customers customer)
        {
            if (ModelState.IsValid)
            {
                // Check age validation
                int minimumAge = 21;
                DateTime today = DateTime.Today;
                int age = today.Year - customer.DateOfBirth.Year;

                if (today < customer.DateOfBirth.AddYears(age))
                {
                    age--;
                }

                if (age < minimumAge)
                {
                    ModelState.AddModelError("DateOfBirth", $"Must be at least {minimumAge} years old.");
                    return View("Signup", customer);
                }

                string connectionString = this.Configuration.GetConnectionString("DefaultConnection");

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))

                    {
                        conn.Open();

                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT * FROM customers WHERE email = @Email OR Contact = @Contact OR UserId=@UserID";
                            cmd.Parameters.AddWithValue("@Email", customer.Email);
                            cmd.Parameters.AddWithValue("@Contact", customer.Contact);
                            cmd.Parameters.AddWithValue("@UserID", customer.UserId);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    ViewBag.Message = "User already exists/User ID in use!";
                                    return View("Signup");
                                }
                                reader.Close();

                            }
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


                                /*cmd2.CommandText = "SELECT userid,name FROM customers WHERE mobileNo = @username";
                               
                                using (var reader2 = cmd2.ExecuteReader())
                                {
                                    if (reader2.Read())
                                    {

                                        int userId = reader2.GetInt32("id");
                                        string userName = reader2.GetString("name");

                                        HttpContext.Session.SetInt32("UserId", userId);
                                        HttpContext.Session.SetString("name", userName);

                                    }
                                }

                                */
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred:");
                    Console.WriteLine("Error Message: " + ex.Message);
                    Console.WriteLine("Stack Trace: " + ex.StackTrace);
                    return View("Error");
                }

            }
            else
            { return View("Signup"); }

            // Redirect to Dashboard or any other desired page
            return RedirectToAction("Login");
        }

        public ActionResult Dashboard()
        {

            return View();
        }
        public IActionResult Logout()
        {
            try
            {
                // Clear the user's session data
                HttpContext.Session.Clear();

                // Redirect the user to the login page or any other desired page
                return RedirectToAction("Index","Home");
            }
            catch (Exception)
            {

                return View("Error"); 
            }
        }
    }
}