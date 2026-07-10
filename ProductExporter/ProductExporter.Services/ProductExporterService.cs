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

    public DataTable GetDataFromMySql()
    {
        string sqlCommand = "select * from SelectAllCategoryWithProduct_V";

        try
        {
            if (sqlCommand == null)
                throw new ArgumentNullException("SqlCommand can't be null", nameof(sqlCommand));

            _dbConnection.Open();
            var dataTable = new DataTable();
            var adapter = new SqlDataAdapter(sqlCommand, (SqlConnection)_dbConnection);

            adapter.Fill(dataTable);
            return dataTable;
        }
        finally
        {

            if (_dbConnection.State != ConnectionState.Closed)
                _dbConnection.Close();
        }
    }
}
