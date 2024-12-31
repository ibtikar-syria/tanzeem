using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Tanzeem.Data;

/* This is used if database provider does't define
 * ITanzeemDbSchemaMigrator implementation.
 */
public class NullTanzeemDbSchemaMigrator : ITanzeemDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
