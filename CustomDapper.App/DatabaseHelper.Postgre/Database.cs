using DatabaseHelper.Common;
using Npgsql;

namespace DatabaseHelper.Postgre;

public sealed class Database : DatabaseCommon<NpgsqlConnection, NpgsqlCommand>
{
    public Database(string connectionString, bool autoConnectionOpen = true)
        : base(connectionString, autoConnectionOpen)
    {
    }
}
