using Microsoft.Data.SqlClient;
using ProductImporter.Services;

namespace ProductImporter.App;

internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            CsvFileDataReader dataReader = new CsvFileDataReader(new FileInfo("D:\\Products.txt"));
            var connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ProductsCatalogue;Integrated Security=True;Encrypt=True");
            ProductImporterService importerService = new ProductImporterService(connection, dataReader);
            importerService.OnImportStarted += () => Console.WriteLine("Import started.");
            importerService.OnImportProgress += () => Console.Write($"\rImport progress");
            importerService.OnImportCompleted += () => Console.WriteLine("\nImport completed successfully.");
            importerService.OnImportError += () => Console.WriteLine("\nAn error occurred during import.");
            importerService.ImportData();

            foreach (var category in dataReader.GetData())
            {
                Console.WriteLine($"\nCategory: {category.Name}");
                foreach (var product in category.Products)
                {
                    Console.WriteLine($"  Product: {product.Name}, Price: {product.Price}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return;
        }
    }
}