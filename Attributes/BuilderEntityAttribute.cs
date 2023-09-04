using System.Data;

namespace core_data_provider.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BuilderEntityAttribute : Attribute
{
    public Type Type { get; }
    public string EntityName { get; set; }

    public BuilderEntityAttribute(Type type, string EntityName)
    {
        Type = type;
        this.EntityName = EntityName;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class SqlPropertyAttribute : Attribute
{
    // public Type Type { get; }
    public string Name { get; set; }
    public SqlDbType SqlDbType { get; set; }
    public bool IsEnum { get; set; }
    public Type? EnumType { get; set; }

    //public SqlPropertyAttribute(string columnName, SqlDbType sqlDbType)
    //{
    //    // Type = type;
    //    Name = columnName;
    //    SqlDbType = sqlDbType;
    //    IsEnum = false;
    //    EnumType = null;
    //}

    public SqlPropertyAttribute(string columnName, SqlDbType sqlDbType, bool isEnum = false, Type? enumType = null)
    {
        // Type = type;
        Name = columnName;
        SqlDbType = sqlDbType;
        IsEnum = isEnum;
        EnumType = enumType;
    }
}