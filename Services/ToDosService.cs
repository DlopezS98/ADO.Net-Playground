using core_data_provider.Entities;
using core_data_provider.Repositories;
using System.Data.SqlClient;

namespace core_data_provider.Services;

public class ToDosService
{
    private readonly ToDosRepository _toDosRepository;
    public ToDosService(SqlConnection connection)
    {
        _toDosRepository = new ToDosRepository(connection);
    }

    public async Task<ToDo> CreateAsync()
    {
        Console.Clear();
        Guid id = Guid.NewGuid();
        Console.WriteLine("Creating a ToDo...");
        Console.WriteLine("Title: ");
        string title = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("Description: ");
        string? description = Console.ReadLine() ?? null;
        Console.WriteLine("Status: ");
        string statusStr = Console.ReadLine() ?? string.Empty;

        ToDoStatus status = (ToDoStatus)Enum.Parse(typeof(ToDoStatus), statusStr);
        DateTime createdAt = DateTime.UtcNow;

        ToDo toDo = new()
        {
            Id = id,
            Title = title,
            Description = description,
            Status = status,
            Done = false,
            CreatedAt = createdAt
        };

        await _toDosRepository.CreateAsync(toDo);
        return toDo;
    }

    public async Task<List<ToDo>> GetAll()
    {
        return await _toDosRepository.GetAll();
    }

    public Task<ToDo?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }
}
