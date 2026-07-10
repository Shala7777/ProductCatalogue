namespace ProductExporter.Models
{
    public sealed class Product
    {
        public int Code { get; set; }
        public string Name { get; set; } = null!;
        public float Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }
    }
}
