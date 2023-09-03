using System.Data;
using core_data_provider.Attributes;
using core_data_provider.Entities;

namespace core_data_provider;

public enum ToDoStatus
{
    InProgress,
    Completed,
    NotStarted
}

[BuilderEntity(typeof(ToDo), "ToDos")]
public class ToDo : BaseEntity
{
    [SqlProperty("Title", SqlDbType.NVarChar)]
    public required string Title { get; set; }
    [SqlProperty("Description", SqlDbType.NVarChar)]
    public string? Description { get; set; }
    [SqlProperty("Done", SqlDbType.Bit)]
    public bool Done { get; set; }
    [SqlProperty("Status", SqlDbType.NVarChar, true, typeof(ToDoStatus))]
    public ToDoStatus Status { get; set; }
    [SqlProperty("StartedAt", SqlDbType.DateTime)]
    public DateTime? StartedAt { get; set; }
    [SqlProperty("CreatedAt", SqlDbType.DateTime)]
    public DateTime CreatedAt { get; set; }
    [SqlProperty("UpdatedAt", SqlDbType.DateTime)]
    public DateTime? UpdatedAt { get; set; }
}