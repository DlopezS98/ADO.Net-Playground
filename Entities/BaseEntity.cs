using core_data_provider.Attributes;
using System.Data;

namespace core_data_provider.Entities;

public class BaseEntity {
  [SqlProperty("Id", SqlDbType.UniqueIdentifier)]
  public Guid Id { get; set; }
}