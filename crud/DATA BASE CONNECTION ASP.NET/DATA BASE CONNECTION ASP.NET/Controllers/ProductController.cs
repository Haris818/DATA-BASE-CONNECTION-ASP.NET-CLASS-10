using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Mvc;
using DATA_BASE_CONNECTION_ASP.NET.Models;
using System.Collections.Generic;
using System.Web.Security; // For using authentication cookies

public class ProductController : Controller
{
    private string connectionString = ConfigurationManager.ConnectionStrings["TestDbConnection"].ConnectionString;

    // GET: Product/Index
    public ActionResult Index()
    {
        // Check if the user is authenticated (i.e., logged in)
        if (Session["UserEmail"] == null)
        {
            return RedirectToAction("Login"); // Redirect to Login if user is not logged in
        }

        var products = new List<Product>();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT Id, Name, Price FROM information";
            SqlCommand cmd = new SqlCommand(query, conn);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString(),
                    Price = Convert.ToDecimal(reader["Price"])
                });
            }
            conn.Close();
        }

        return View(products);
    }

    // GET: Product/Signup
    public ActionResult Signup()
    {
        return View();
    }

    // POST: Product/Signup
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Signup(User user)
    {
        if (ModelState.IsValid)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Users (Email, Password) VALUES (@Email, @Password)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password); // In production, consider hashing passwords

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                return RedirectToAction("Login");  // Redirect to Login page after successful signup
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
            }
        }

        return View(user);
    }

    // GET: Product/Login
    public ActionResult Login()
    {
        return View();
    }

    // POST: Product/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(User user)
    {
        if (ModelState.IsValid)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        // Successfully logged in, store session or user info if needed
                        Session["UserEmail"] = user.Email;  // Store user email in session (or use other session variables if needed)
                        return RedirectToAction("Index"); // Redirect to Product Index page after successful login
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Invalid email or password.";
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
            }
        }

        return View(user);
    }

    // GET: Product/Logout
    public ActionResult Logout()
    {
        // Clear session or authentication cookies
        Session.Clear();  // Clear all session data
        Session.Abandon();  // Abandon the session to completely clean it up

        return RedirectToAction("Login");  // Redirect to Login page after logout
    }

    // GET: Product/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO information (Name, Price) VALUES (@Name, @Price)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Price", product.Price);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
            }
        }

        return View(product);  // Return the view with validation errors
    }

    // GET: Product/Edit/{id}
    public ActionResult Edit(int id)
    {
        Product product = null;

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT Id, Name, Price FROM information WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                product = new Product
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString(),
                    Price = Convert.ToDecimal(reader["Price"])
                };
            }
            conn.Close();
        }

        if (product == null)
        {
            return HttpNotFound();  // Return 404 if product not found
        }

        return View(product);  // Return the Edit view with the product details
    }

    // POST: Product/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, Product product)
    {
        if (ModelState.IsValid)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE information SET Name = @Name, Price = @Price WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Price", product.Price);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
                return View(product);
            }
        }

        return View(product);
    }

    // GET: Product/Delete/{id}
    public ActionResult Delete(int id)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM information WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            return RedirectToAction("Index"); // Redirect to the Index page after deletion
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Error: " + ex.Message;
            return RedirectToAction("Index"); // Redirect to Index in case of error
        }
    }
}
