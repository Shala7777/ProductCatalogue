using System.Data;
using System.Data.Common;

namespace DatabaseHelper.Common;

public abstract class DatabaseCommon<TConnection, TCommand> : IDisposable
    where TConnection : IDbConnection, new()
    where TCommand : IDbCommand
{
    protected readonly string _connectionString;
    protected readonly bool _autoConnectionOpen;
    protected bool _disposed;
    protected TConnection? _connection;
    protected IDbTransaction? _transaction;

    protected DatabaseCommon(string connectionString, bool autoConnectionOpen = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;
        _autoConnectionOpen = autoConnectionOpen;
    }

    public IDbTransaction? Transaction => _transaction;

    public TConnection GetConnection()
    {
        if (_connection == null)
        {
            _connection = new TConnection();
            _connection.ConnectionString = _connectionString;
        }
        return _connection;
    }

    public TConnection OpenConnection()
    {
        if (GetConnection().State != ConnectionState.Open)
            _connection!.Open();
        return _connection!;
    }

    public void CloseConnection()
    {
        if (_connection?.State != ConnectionState.Closed)
            _connection!.Close();
    }

    public void BeginTransaction()
    {
        if (_transaction != null)
            throw new InvalidOperationException("A transaction is already in progress.");
        _transaction = (DbTransaction)OpenConnection().BeginTransaction();
    }

    public void CommitTransaction() => HandleTransaction(() => _transaction!.Commit());

    public void RollbackTransaction() => HandleTransaction(() => _transaction!.Rollback());

    public int ExecuteNonQuery(string commandText, CommandType commandType, params IDbDataParameter[] parameters)
    {
        using IDbCommand command = GetCommand(commandText, commandType, parameters);
        if (_autoConnectionOpen && command.Connection!.State != ConnectionState.Open)
            OpenConnection();
        return command.ExecuteNonQuery();
    }

    public int ExecuteNonQuery(string commandText, params IDbDataParameter[] parameters) => ExecuteNonQuery(commandText, CommandType.Text, parameters);

    public object? ExecuteScalar(string commandText, CommandType commandType, params IDbDataParameter[] parameters)
    {
        using IDbCommand command = GetCommand(commandText, commandType, parameters);
        if (_autoConnectionOpen && command.Connection!.State != ConnectionState.Open)
            OpenConnection();
        return command.ExecuteScalar();
    }

    public object? ExecuteScalar(string commandText, params IDbDataParameter[] parameters) => ExecuteScalar(commandText, CommandType.Text, parameters);

    public IDataReader ExecuteReader(string commandText, CommandType commandType, params IDbDataParameter[] parameters)
    {
        IDbCommand command = GetCommand(commandText, commandType, parameters);
        if (_autoConnectionOpen && command.Connection!.State != ConnectionState.Open)
            OpenConnection();
        return command.ExecuteReader();
    }

    public IDataReader ExecuteReader(string commandText, params IDbDataParameter[] parameters) => ExecuteReader(commandText, CommandType.Text, parameters);

    public DataTable GetDataTable(string commandText, CommandType commandType, params IDbDataParameter[] parameters)
    {
        using IDbCommand command = GetCommand(commandText, commandType, parameters);
        if (_autoConnectionOpen && command.Connection!.State != ConnectionState.Open)
            OpenConnection();
        DataTable dataTable = new DataTable();
        dataTable.Load(ExecuteReader(commandText, commandType, parameters));
        return dataTable;
    }

    public DataTable GetDataTable(string commandText, params IDbDataParameter[] parameters) => GetDataTable(commandText, CommandType.Text, parameters);

    protected IDbCommand GetCommand(string commandText, CommandType commandType, params IDbDataParameter[] parameters)
    {
        IDbCommand command = (IDbCommand)GetConnection().CreateCommand();
        command.CommandText = commandText;
        command.CommandType = commandType;

        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        command.Transaction = _transaction;
        return command;
    }

    protected IDbCommand GetCommand(string commandText, params DbParameter[] parameters) => GetCommand(commandText, CommandType.Text, parameters);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _connection?.Dispose();
            _transaction?.Dispose();
        }

        _disposed = true;
    }

    private void HandleTransaction(Action action)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No transaction is in progress.");
        action();
        _transaction.Dispose();
        _transaction = null;
    }

    ~DatabaseCommon()
    {
        Dispose(false);
    }
}
