using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;

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
    public event Action<Exception> OnImportError;

    public void ImportData()
    {
        OnImportStarted?.Invoke();
        IDbTransaction? transaction = null;

        try
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
                transaction = _dbConnection.BeginTransaction();
            }

            using var command = _dbConnection.CreateCommand();
            command.CommandText = "sp_InsertCatalogueItem";
            command.CommandType = CommandType.StoredProcedure;
            command.Transaction = transaction;

            foreach (var category in _dataReader.GetData())
            {
                ProcessCategory(command, category);
            }
            transaction?.Commit();
        }
        catch (Exception ex)
        {
            transaction?.Rollback();
            OnImportError?.Invoke(ex);
            throw;
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
        OnImportCompleted?.Invoke();
    }

    private void GetParameters(IDbCommand command)
    {
        string[] parameterNames =
        {
            "@CategoryName",
            "@CategoryIsDeleted",
            "@ProductCode",
            "@ProductName",
            "@ProductPrice",
            "@ProductQuantity",
            "@ProductIsDeleted"
        };

        foreach (var name in parameterNames)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            command.Parameters.Add(parameter);
        }
    }

    private void ProcessCategory(IDbCommand command, Models.Category category)
    {
        foreach (var product in category.Products)
        {
            GetParameters(command);
            ((IDbDataParameter)command.Parameters["@CategoryName"]).Value = category.Name;
            ((IDbDataParameter)command.Parameters["@CategoryIsDeleted"]).Value = category.IsActive;
            ((IDbDataParameter)command.Parameters["@ProductCode"]).Value = product.Code;
            ((IDbDataParameter)command.Parameters["@ProductName"]).Value = product.Name;
            ((IDbDataParameter)command.Parameters["@ProductPrice"]).Value = product.Price;
            ((IDbDataParameter)command.Parameters["@ProductQuantity"]).Value = product.Quantity;
            ((IDbDataParameter)command.Parameters["@ProductIsDeleted"]).Value = product.IsActive;

            command.ExecuteNonQuery();
            command.Parameters.Clear();

            OnImportProgress?.Invoke();
        }
    }
}