using Microsoft.Data.SqlClient;
using System.Data;

namespace ProductExporter.Services;

public class ProductExporterService : Interfaces.IDataTableReader
{
    private readonly IDbConnection _dbConnection;

    public ProductExporterService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
    }

    public DataTable GetData()
    {
        string sqlCommand = "select * from SelectAllCategoryWithProduct_V";

        if (sqlCommand == null)
            throw new ArgumentNullException("SqlCommand can't be null", nameof(sqlCommand));
         
        _dbConnection.Open();
        using var dataTable = new DataTable();
        using var adapter = new SqlDataAdapter(sqlCommand, (SqlConnection)_dbConnection);

        adapter.Fill(dataTable);

        if (_dbConnection.State != ConnectionState.Closed)
            _dbConnection.Close();
        
        return dataTable;
    }
}
