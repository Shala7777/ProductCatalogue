using System.Data;

namespace ProductExporter.Services.Interfaces
{
    public interface IDataTableReader
    {
        DataTable GetDataFromMySql();
    }
}
