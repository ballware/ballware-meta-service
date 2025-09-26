using System.Data;

namespace Ballware.Meta.Data;

public interface IMetaDbConnectionFactory
{
    string Provider { get; }
    string ConnectionString { get; }
}