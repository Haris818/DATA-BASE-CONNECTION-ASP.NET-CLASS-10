namespace DATA_BASE_CONNECTION_ASP.NET.Models
{
    public class Product
    {
        public int Id { get; set; }  // This is usually auto-incremented in SQL Server
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
