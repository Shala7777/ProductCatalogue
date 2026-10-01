using Dapper;
using System.Data;
using System.Data.Common;

namespace CustomDapper;

public class DbAdapter<TEntity, TConnection>
{
    private readonly IDbConnection _dbConnection;

    public DbAdapter(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
    }

    public IEnumerable<TEntity> ExecuteQuery<TEntity>() where TEntity : new()
    {
        using var command = _dbConnection.CreateCommand();

        command.CommandText = "select * from products";
        command.CommandType = CommandType.Text;

        using var reader = command.ExecuteReader();

        var properties = typeof(TEntity).GetProperties();

        while (reader.Read())
        {
            TEntity item = new TEntity();

            foreach (var property in properties)
            {
                var value = reader[property.Name];

                if (value != DBNull.Value)
                {
                    property.SetValue(item, value);
                }
            }

            yield return item;
        }
    }
}
