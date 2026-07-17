using Microsoft.Data.SqlClient;
using ProductExporter.DataWriter;
using ProductExporter.Services;

namespace ProductExporter.App;

internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            var connecton = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;Encrypt=True");
            var dataExporter = new ProductExporterService(connecton);
            var dataWriter = new CsvFileDataWriter(new FileInfo(@"D:\Nor.txt"), dataExporter);
            dataWriter.WriteDataToFile();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
