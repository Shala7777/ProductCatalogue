using DatabaseHelper.Common;
using Microsoft.Data.SqlClient;

namespace DatabaseHelper.SqlServer;

public sealed class Database : DatabaseCommon<SqlConnection, SqlCommand>
{
    public Database(string connectionString, bool autoConnectionOpen = true)
        : base(connectionString, autoConnectionOpen)
    {

    }
}
