using ProductImporter.Models;

namespace ProductImporter.Services.Interfaces;

public interface IDataReader
{
    IEnumerable<Category> GetData();
}