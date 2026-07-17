namespace ProductImporter.Models
{
    public class Category
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
