using System.Data.SqlClient;
using System.Text;
using core_data_provider.Attributes;
using core_data_provider.Entities;
using System.Reflection;
using System.Data;

class CommandSettings
{
    public List<string> Parameters { get; set; } = new();
    public List<string> Columns { get; set; } = new();
}

class ColumnSetting
{
    public required string Name { get; set; }
    public string ParameterName { get => $"@{Name}"; }
    public SqlDbType SqlDbType { get; set; }
    public object? Value { get; set; }
}

public sealed class SqlOperationsBuilder
{
    public SqlOperationsBuilder<T> From<T>(T entity) where T : BaseEntity
    {
        return new SqlOperationsBuilder<T>(entity);
    }
}

public class SqlOperationsBuilder<T> where T : BaseEntity
{
    private readonly T entity;
    public SqlOperationsBuilder(T entity)
    {
        this.entity = entity;
    }

    public SqlCommand GetInsertCommand()
    {
        string entityName = GetEntityName(entity);
        List<ColumnSetting> settings = GetColumnSettings(entity);
        string sqlQuery = GetInsetQuery(entityName, settings);
        SqlCommand sqlCommand = new SqlCommand(sqlQuery);
        sqlCommand.Parameters.AddRange(GetSqlParameters(settings));
        return sqlCommand;
    }

    private string GetInsetQuery(string entityName, List<ColumnSetting> settings)
    {

        StringBuilder builder = new StringBuilder();
        List<string> columns = settings.Select(setting => setting.Name).ToList();
        List<string> parameters = settings.Select(setting => setting.ParameterName).ToList();
        builder.Append("INSERT INTO ")
            .Append($"[{entityName}] (")
            .AppendJoin(',', columns)
            .Append(") ")
            .Append("VALUES (")
            .AppendJoin(',', parameters)
            .Append(");");

        return builder.ToString();
    }

    private string GetEntityName(T entity)
    {
        Type type = entity.GetType();
        BuilderEntityAttribute? attribute = type
            .GetCustomAttributes<BuilderEntityAttribute>()
            .FirstOrDefault();

        return attribute?.EntityName ?? type.Name;
    }

    private List<ColumnSetting> GetColumnSettings(T entity)
    {
        Type entityType = entity.GetType();
        List<PropertyInfo> properties = entityType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToList();
        var attributes = properties
            .Where(type => type.GetCustomAttribute<SqlPropertyAttribute>() is not null)
            .Select(type => new
            {
                ProperyName = type.Name,
                Attribute = type.GetCustomAttribute<SqlPropertyAttribute>()!
            })
            .ToList();

        return attributes.Select(attr =>
        {
            object? value = entityType.GetProperty(attr.ProperyName)?.GetValue(entity) ?? null;
            if (attr.Attribute.IsEnum && value is not null)
            {
                value = Enum.GetName(attr.Attribute.EnumType!, value);
            }

            return new ColumnSetting
            {
                Name = attr.Attribute.Name,
                SqlDbType = attr.Attribute.SqlDbType,
                Value = value
            };
        }).ToList();
    }

    private SqlParameter[] GetSqlParameters(List<ColumnSetting> settings)
    {
        List<SqlParameter> parameters = new();
        foreach (ColumnSetting setting in settings)
        {
            bool isNullable = setting.Value is null;
            SqlParameter parameter = new()
            {
                ParameterName = setting.ParameterName,
                Value = setting.Value ?? DBNull.Value,
                SqlDbType = setting.SqlDbType,
                IsNullable = isNullable
            };

            parameters.Add(parameter);
        }

        return parameters.ToArray();
    }
}