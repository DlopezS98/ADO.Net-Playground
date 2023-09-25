using core_data_provider.Entities;
using core_data_provider.Repositories;
using System.Data.SqlClient;

namespace core_data_provider.Services;

public class ToDosService
{
    private readonly ToDosRepository _toDosRepository;
    public ToDosService(ToDosRepository repository)
    {
        _toDosRepository = repository;
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

    public Task<List<ToDo>> GetAll()
    {
        return _toDosRepository.GetAll();
    }

    public Task<ToDo?> GetById(Guid id)
    {
        return _toDosRepository.GetByIdAsync(id);
    }

    public async Task DeleteById()
    {
        Console.WriteLine("Enter de Id: ");
        Guid id  = Guid.Parse(Console.ReadLine() ?? string.Empty);

        ToDo? toDo = await _toDosRepository.GetByIdAsync(id);
        if (toDo == null)
        {
            Console.WriteLine("\n ToDo with Id: {0} Not found", id);
            return;
        }

        await _toDosRepository.DeleteAsync(id);
    }
}
