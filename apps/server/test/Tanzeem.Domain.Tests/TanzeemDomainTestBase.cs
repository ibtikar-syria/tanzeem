using Volo.Abp.Modularity;

namespace Tanzeem;

/* Inherit from this class for your domain layer tests. */
public abstract class TanzeemDomainTestBase<TStartupModule> : TanzeemTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
