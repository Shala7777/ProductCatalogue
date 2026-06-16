using Microsoft.Data.SqlClient;
using System.Data;

namespace ProductImporter.Services;

public sealed class ProductImporterService
{
    private readonly IDbConnection _dbConnection;
    private readonly Interfaces.IDataReader _dataReader;

    public ProductImporterService(IDbConnection dbConnection, Interfaces.IDataReader dataReader)
    {
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        _dataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
    }

    public event Action OnImportStarted;
    public event Action OnImportProgress;
    public event Action OnImportCompleted;
    public event Action OnImportError;

    public void ImportData()
    {
        using var connection = _dbConnection;
        using var command = connection.CreateCommand();

        command.CommandText = "sp_InsertCatalogueItem";
        command.CommandType = CommandType.StoredProcedure;

        connection.Open();
        OnImportStarted?.Invoke();


        foreach (var category in _dataReader.GetData())
        {
            category.Products.ToList().ForEach(product =>
            {
                command.Parameters.Add(new SqlParameter("@CategoryName", DbType.String) { Value = category.Name });
                command.Parameters.Add(new SqlParameter("@CategoryIsDeleted", DbType.Boolean) { Value = category.IsActive });
                command.Parameters.Add(new SqlParameter("@ProductCode", DbType.String) { Value = product.Code });
                command.Parameters.Add(new SqlParameter("@ProductName", DbType.String) { Value = product.Name });
                command.Parameters.Add(new SqlParameter("@ProductPrice", DbType.Decimal) { Value = product.Price });
                command.Parameters.Add(new SqlParameter("@ProductQuantity", DbType.Single) { Value = product.Quantity });
                command.Parameters.Add(new SqlParameter("@ProductIsDeleted", DbType.Boolean) { Value = product.IsActive });

                OnImportProgress?.Invoke();
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            });
        }
        OnImportCompleted?.Invoke();
    }
}