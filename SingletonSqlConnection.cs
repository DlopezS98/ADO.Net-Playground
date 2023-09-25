using System.Data;
using System.Data.SqlClient;

namespace core_data_provider;

public class SingletonSqlConnection
{
    private static SingletonSqlConnection _instance;

    private readonly SqlConnection connection;

    private SingletonSqlConnection(string connectionString)
    {
        connection = new SqlConnection(connectionString);
        connection.Open();
    }

    public static SingletonSqlConnection GetInstance(string connectionString)
    {
        _instance ??= new SingletonSqlConnection(connectionString);
        return _instance;
    }

    public void SetSqlConnection(SqlCommand command)
    {
        command.Connection = connection;
    }

    public SqlCommand CreateCommand(string query)
    {
        return new SqlCommand(query, connection);
    }

    public SqlDataAdapter CreateDataApdapter(string query)
    {
        return new SqlDataAdapter(query, connection);
    }

    public DataTable ExecuteQueryCommand(string sql)
    {
        DataTable dt = new DataTable();
        OpenConnection();
        SqlCommand command = new SqlCommand(sql, connection);
        command.CommandType = CommandType.Text;
        SqlDataReader reader = command.ExecuteReader();
        dt.Load(reader);
        command.Dispose();
        return dt;
    }

    public async Task<DataTable> ExecuteQueryCommandAsync(string sql)
    {
        DataTable dt = new DataTable();
        OpenConnection();
        SqlCommand command = new SqlCommand(sql, connection);
        command.CommandType = CommandType.Text;
        SqlDataReader reader = await command.ExecuteReaderAsync();
        dt.Load(reader);
        command.Dispose();
        return dt;
    }

    public void OpenConnection()
    {
        if (connection.State == ConnectionState.Open) return;

        connection.Open();
    }

    public void CloseConnection()
    {
        if (connection.State == ConnectionState.Closed) return;

        connection.Close();
    }

    public void Dispose()
    {
        if (connection is null) return;

        connection.Dispose();
    }
}
