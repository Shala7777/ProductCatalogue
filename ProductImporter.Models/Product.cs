namespace ProductImporter.Models
{
    public class Product
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public float Quantity { get; set; }
        public bool IsActive { get; set; }
    }
}
