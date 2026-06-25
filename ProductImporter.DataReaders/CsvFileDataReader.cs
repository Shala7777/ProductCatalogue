using ProductImporter.Models;
using ProductImporter.Services.Interfaces;

public sealed class CsvFileDataReader : IDataReader
{
    private readonly string _filePath;

    public CsvFileDataReader(FileInfo fileInfo)
    {
        if (fileInfo == null)
            throw new ArgumentNullException(nameof(fileInfo));
        if (string.IsNullOrWhiteSpace(fileInfo.FullName))
            throw new ArgumentException("File path cannot be null or whitespace.", nameof(fileInfo.FullName));

        _filePath = fileInfo.FullName;
    }

    public IEnumerable<Category> GetData()
    {
        if (!File.Exists(_filePath))
        {
            var fileStream = new FileStream(_filePath, FileMode.OpenOrCreate);
        }

        Dictionary<string, Category> categoryDictionary = new Dictionary<string, Category>();

        using var reader = new StreamReader(_filePath);
        string line;

        while ((line = reader.ReadLine()!) != null)
        {
            string[] tokens = line.Split('\t');
            ValidateTokens(tokens);

            var categoryName = tokens[(int)Indexes.CategoryNameIndex];
            var category = GetOrCreateCategory(categoryDictionary, categoryName, tokens);
            AddProductFromTokens(category, tokens);
        }

        return categoryDictionary.Values;
    }

    private static void AddProductFromTokens(Category category, IReadOnlyList<string> tokens)
    {
        category.Products.Add(new Product
        {
            Code = tokens[(int)Indexes.productCodeIndex],
            Name = tokens[(int)Indexes.ProductNamaIndex],
            Price = decimal.Parse(tokens[(int)Indexes.ProductQuantityIndex]),
            Quantity = float.Parse(tokens[(int)Indexes.ProductPriceIndex]),
            IsActive = tokens[(int)Indexes.ProductIsDeletedIndex] == "1"
        });
    }

    private static Category GetOrCreateCategory(IDictionary<string, Category> categories, string categoryName, IReadOnlyList<string> tokens)
    {
        if (!categories.TryGetValue(categoryName, out var category))
        {
            category = new Category
            {
                Name = categoryName,
                IsActive = tokens[(int)Indexes.CategoryIsDeletedIndex] == "1"
            };
            categories.Add(categoryName, category);
        }

        return category;
    }

    private void ValidateTokens(string[] tokens)
    {
        if (tokens.Length != (int)Indexes.ExpectedTokenCount)
            throw new FormatException($"Invalid number of tokens. Expected {(int)Indexes.ExpectedTokenCount} but got {tokens.Length}.");
    }
}