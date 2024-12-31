using System.Threading.Tasks;

namespace Tanzeem.Data;

public interface ITanzeemDbSchemaMigrator
{
    Task MigrateAsync();
}
