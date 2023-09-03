namespace core_data_provider;

public enum ToDoStatus
{
    InProgress,
    Completed,
    NotStarted
}
public class ToDo
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool Done { get; set; }
    public ToDoStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}