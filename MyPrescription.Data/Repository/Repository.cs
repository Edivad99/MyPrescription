using System.Data.Common;
using MySql.Data.MySqlClient;

namespace MyPrescription.Data.Repository;

public abstract class Repository
{
    private readonly string ConnectionString;

    protected Repository(string connectionString) => ConnectionString = connectionString;

    protected DbConnection GetDbConnection() => new MySqlConnection(ConnectionString);
}
