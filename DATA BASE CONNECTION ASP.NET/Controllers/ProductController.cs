using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Mvc;
using DATA_BASE_CONNECTION_ASP.NET.Models;  // Make sure to use the correct namespace

public class ProductController : Controller
{
    private string connectionString = ConfigurationManager.ConnectionStrings["TestDbConnection"].ConnectionString;

    // GET: Product/Create
    public ActionResult Create()
    {
        return View();  // Return to the Create view
    }

    // POST: Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]  // Protect against CSRF attacks
    public ActionResult Create(Product product)
    {
        if (ModelState.IsValid)  // Check if the model is valid
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO information (id, Name, Price) VALUES (@id,@Name, @Price)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@id", product.Name);  // Bind Name parameter
                    cmd.Parameters.AddWithValue("@Name", product.Name);  // Bind Name parameter
                    cmd.Parameters.AddWithValue("@Price", product.Price);  // Bind Price parameter

                    conn.Open();
                    cmd.ExecuteNonQuery();  // Execute the query to insert the data
                    conn.Close();
                }

                return RedirectToAction("Index");  // Redirect to the Index view after successful insertion
            }
            catch (Exception ex)
            {
                // If an error occurs, set the error message to ViewBag.ErrorMessage
                ViewBag.ErrorMessage = "Error: " + ex.Message;
                return View(product);  // Return the view with the error message
            }
        }

        return View(product);  // Return the view with validation errors
    }
}
