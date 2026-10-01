using DatabaseHelper.Common;
using Oracle.ManagedDataAccess.Client;

namespace DatabaseHelper.Oracle;

public sealed class Database : DatabaseCommon<OracleConnection, OracleCommand>
{
    public Database(string connectionString, bool autoConnectionOpen = true)
        : base(connectionString, autoConnectionOpen)
    {

    }
}
