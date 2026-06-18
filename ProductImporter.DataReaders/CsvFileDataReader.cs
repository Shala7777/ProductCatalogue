using ProductImporter.Models;
using ProductImporter.Services.Interfaces;

public sealed class CsvFileDataReader : IDataReader
{
    private const int ExpectedTokenCount = 7;
    private const int CategoryNameIndex = 0;
    private const int CategoryIsDeletedIndex = 1;

    private readonly string _filePath;

    public CsvFileDataReader(FileInfo fileInfo)
    {
        if (fileInfo == null)
            throw new ArgumentNullException(nameof(fileInfo));
        if (string.IsNullOrWhiteSpace(fileInfo.FullName))
            throw new ArgumentException("File path cannot be null or whitespace.", nameof(fileInfo.FullName));
        if (!File.Exists(fileInfo.FullName))
            throw new FileNotFoundException("The specified file was not found.", fileInfo.FullName);

        _filePath = fileInfo.FullName;
    }

    public IEnumerable<Category> GetData()
    {
        Dictionary<string, Category> categoryDictionary = new Dictionary<string, Category>();

        using var reader = new StreamReader(_filePath);
        string line;

        while ((line = reader.ReadLine()!) != null)
        {
            string[] tokens = line.Split('\t');
            ValidateTokens(tokens);

            var categoryName = tokens[CategoryNameIndex];
            var category = GetOrCreateCategory(categoryDictionary, categoryName, tokens);
            AddProductFromTokens(category, tokens);
        }

        return categoryDictionary.Values;
    }

    private static void AddProductFromTokens(Category category, IReadOnlyList<string> tokens)
    {
        category.Products.Add(new Product
        {
            Code = tokens[2],
            Name = tokens[3],
            Price = decimal.Parse(tokens[4]),
            Quantity = float.Parse(tokens[5]),
            IsActive = tokens[6] == "1"
        });
    }

    private static Category GetOrCreateCategory(IDictionary<string, Category> categories, string categoryName, IReadOnlyList<string> tokens)
    {
        if (!categories.TryGetValue(categoryName, out var category))
        {
            category = new Category
            {
                Name = categoryName,
                IsActive = tokens[CategoryIsDeletedIndex] == "1"
            };
            categories.Add(categoryName, category);
        }

        return category;
    }

    private void ValidateTokens(string[] tokens)
    {
        if (tokens.Length != ExpectedTokenCount)
            throw new FormatException($"Invalid number of tokens. Expected {ExpectedTokenCount} but got {tokens.Length}.");
    }
}