using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using core_data_provider.Builders;
using core_data_provider.Entities;

namespace core_data_provider.Repositories;

public class ToDosRepository
{
    private readonly SqlConnection _connection;
    public ToDosRepository(SqlConnection sqlConnection)
    {
        _connection = sqlConnection;
    }

    public async Task<ToDo> CreateAsync(ToDo toDo)
    {
        SqlDataAdapter adapter = new SqlDataAdapter();
        toDo.Id = Guid.NewGuid();
        SqlCommand cmd = new SqlOperationsBuilder().From(toDo).GetInsertCommand();
        cmd.CommandType = CommandType.Text;
        cmd.Connection = _connection;
        //const string sqlQuery = "INSERT INTO ToDos (Id, Title, Description, Done, Status, CreatedAt)" +
        //    "VALUES (@Id, @Title, @Description, @Done, @Status, @CreatedAt)";
        //// https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/executing-a-command
        //SqlCommand cmd = new SqlCommand(sqlQuery, _connection);
        //SqlParameter[] sqlParameters = GetParameters(toDo);
        //cmd.Parameters.AddRange(sqlParameters);
        adapter.InsertCommand = cmd;
        int rowsAffected = await adapter.InsertCommand.ExecuteNonQueryAsync();
        Console.WriteLine($"{rowsAffected} rows affected.");
        return toDo;
    }

    // TODO: Create local transactions
    // Resource: https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/local-transactions

    // public async Task CreateWithDataSet(ToDo toDo)
    // {
    //     SqlDataAdapter adapter = new SqlDataAdapter();
    //     toDo.Id = Guid.NewGuid();
    //     adapter.Update()
    // }

    public async Task DeleteAsync(Guid id)
    {
        string sqlQuery = "DELETE FROM ToDos WHERE Id = @Id";
        SqlCommand command = new(sqlQuery, _connection);
        command.CommandType = CommandType.Text;
        command.Parameters.Add(new SqlParameter()
        {
            Direction = ParameterDirection.Input,
            ParameterName = "@Id",
            SqlDbType = SqlDbType.UniqueIdentifier,
            Value = id
        });

        int rowsAffected = await command.ExecuteNonQueryAsync();
        command.Dispose();
        Console.WriteLine($"{rowsAffected} rows affected.");
    }

    public async Task<ToDo?> GetByIdAsync(Guid id)
    {
        DataTable dataTable = new DataTable();
        string sqlQuery = "SELECT * FROM ToDos WHERE Id = @Id";
        SqlCommand command = new(sqlQuery, _connection);
        command.CommandType = CommandType.Text;
        command.Parameters.Add(new SqlParameter()
        {
            Direction = ParameterDirection.Input,
            ParameterName = "@Id",
            SqlDbType = SqlDbType.UniqueIdentifier,
            Value = id
        });

        SqlDataReader dataReader = await command.ExecuteReaderAsync();
        dataTable.Load(dataReader);
        command.Dispose();

        return dataTable
            .AsEnumerable()
            .Select(MapEntityFromDataRow)
            .FirstOrDefault();
    }

    public async Task<List<ToDo>> GetAll()
    {
        DataTable dataTable = new DataTable();
        const string sqlQuery = "SELECT * FROM ToDos;";
        SqlCommand command = new SqlCommand(sqlQuery, _connection);
        SqlDataReader reader = await command.ExecuteReaderAsync();
        dataTable.Load(reader);
        command.Dispose();

        return dataTable
            .AsEnumerable()
            .Select(MapEntityFromDataRow)
            .ToList();
    }

    public Task<List<ToDo>> RetrieveDataWithDataAdaptersAsync()
    {
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        const string sqlQuery = "SELECT * FROM ToDos";
        SqlCommand cmd = new SqlCommand(sqlQuery, _connection);
        adapter.SelectCommand = cmd;
        adapter.Fill(dataSet);
        List<ToDo> toDos = new();
        foreach (DataRow row in dataSet.Tables[0].Rows)
        {
            ToDo toDo = MapEntityFromDataRow(row);
            toDos.Add(toDo);
        }

        return Task.FromResult(toDos);
    }

    public List<ToDo> RetrieveDataWithDataAdapterAndQueryingWithLinq()
    {
        // https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/loading-data-into-a-dataset
        DataSet dataSet = new DataSet();
        const string sqlQuery = "SELECT * FROM ToDos;";
        SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, _connection);
        DataTableMapping mapping = adapter.TableMappings.Add("ToDosMapping", "ToDos");
        // Change column names from the datasource
        mapping.ColumnMappings.Add("Name", "Title");

        // https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/dataadapter-datatable-and-datacolumn-mappings
        // Throws an error if the mapping name is not specified and it's default to "Table"
        adapter.Fill(dataSet, "ToDosMapping");

        DataTable toDosDt = dataSet.Tables["ToDos"] ?? throw new Exception("Not Table name 'ToDos' found");
        // var query = from toDo in toDosDt.AsEnumerable()
        //     where toDo.Field<string>("Status") == Enum.GetName(ToDoStatus.Completed)
        //     select new ToDo {
        //         Id = toDo.Field<Guid>("Id"),
        //         Title = toDo.Field<string>("Title") ?? string.Empty, // Title => Name
        //         Status = Enum.Parse<ToDoStatus>(toDo.Field<string>("Status") ?? "NotStarted"),
        //         Description = toDo.Field<string?>("Description"),
        //         CreatedAt = toDo.Field<DateTime>("CreatedAt"),
        //         Done = toDo.Field<bool>("Done"),
        //         StartedAt = toDo.Field<DateTime?>("StartedAt"),
        //         UpdatedAt = toDo.Field<DateTime?>("UpdatedAt")
        //     };

        // var query = toDosDt.Rows
        //     .Cast<DataRow>()
        //     .Where(toDo => toDo.Field<string>("Status") == Enum.GetName(ToDoStatus.Completed))
        //     .Select();
        var query = toDosDt
            .AsEnumerable()
            .Where(toDo => toDo.Field<string>("Status") == Enum.GetName(ToDoStatus.Completed))
            .Select(toDo => new ToDo
            {
                Id = toDo.Field<Guid>("Id"),
                Title = toDo.Field<string>("Title") ?? string.Empty, // Title => Name
                Status = Enum.Parse<ToDoStatus>(toDo.Field<string>("Status") ?? "NotStarted"),
                Description = toDo.Field<string?>("Description"),
                CreatedAt = toDo.Field<DateTime>("CreatedAt"),
                Done = toDo.Field<bool>("Done"),
                StartedAt = toDo.Field<DateTime?>("StartedAt"),
                UpdatedAt = toDo.Field<DateTime?>("UpdatedAt")
            });

        return query.ToList();
    }

    private static T GetValueOrDefault<T>(DataRow row, string index, T defaultValue = default!)
    {
        return !row.IsNull(index) ? (T)row[index] : defaultValue;
    }

    private ToDo MapEntityFromDataRow(DataRow row)
    {
        ToDo toDo = new()
        {
            Id = GetValueOrDefault<Guid>(row, "Id"),
            Title = GetValueOrDefault<string>(row, "Title"),
            Description = GetValueOrDefault<string?>(row, "Description"),
            Done = GetValueOrDefault<bool>(row, "Done"),
            Status = Enum.Parse<ToDoStatus>(GetValueOrDefault<string>(row, "Status")),
            CreatedAt = GetValueOrDefault<DateTime>(row, "CreatedAt"),
            StartedAt = GetValueOrDefault<DateTime?>(row, "StartedAt"),
            UpdatedAt = GetValueOrDefault<DateTime?>(row, "UpdatedAt"),
        };

        return toDo;
    }

    private SqlParameter[] GetParameters(ToDo toDo)
    {
        SqlParameter idParameter = new SqlParameter("@Id", SqlDbType.UniqueIdentifier)
        {
            Direction = ParameterDirection.Input,
            Value = toDo.Id
        };

        SqlParameter titleParameter = new SqlParameter()
        {
            ParameterName = "@Title",
            Direction = ParameterDirection.Input,
            Value = toDo.Title,
            SqlDbType = SqlDbType.NVarChar,
        };

        SqlParameter[] sqlParameters = new SqlParameter[]
        {
            idParameter,
            titleParameter,
            new SqlParameter()
            {
                ParameterName = "@Description",
                Value = toDo.Description,
                SqlDbType = SqlDbType.NVarChar,
            },
            new SqlParameter
            {
                ParameterName = "@Done",
                Value = toDo.Done,
                SqlDbType = SqlDbType.Bit,
            },
            new SqlParameter
            {
                ParameterName = "@Status",
                Value = Enum.GetName(typeof(ToDoStatus), toDo.Status),
                SqlDbType = SqlDbType.NVarChar,
            },
            new SqlParameter
            {
                ParameterName = "@CreatedAt",
                Value = toDo.CreatedAt,
                SqlDbType = SqlDbType.DateTime,
            }
        };

        return sqlParameters;
    }
}
