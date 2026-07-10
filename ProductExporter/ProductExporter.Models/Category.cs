namespace ProductExporter.Models
{
    public sealed class Category
    {
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
