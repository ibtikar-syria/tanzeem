using Volo.Abp.Modularity;

namespace Tanzeem;

public abstract class TanzeemApplicationTestBase<TStartupModule> : TanzeemTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
