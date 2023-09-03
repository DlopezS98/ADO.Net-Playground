using core_data_provider.Attributes;

namespace core_data_provider.Entities;

public class BaseEntity {
  [SqlProperty("Id", System.Data.SqlDbType.UniqueIdentifier)]
  public Guid Id { get; set; }
}