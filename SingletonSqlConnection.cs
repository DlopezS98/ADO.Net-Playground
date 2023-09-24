using System.Data;
using System.Data.SqlClient;

namespace core_data_provider;

public class SingletonSqlConnection
{
    private static SingletonSqlConnection _instance;

    private readonly SqlConnection connection;

    private SingletonSqlConnection()
    {
        connection = new SqlConnection();
        connection.Open();
    }

    public static SingletonSqlConnection Instance
    {
        get
        {
            _instance ??= new SingletonSqlConnection();
            return _instance;
        }
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
