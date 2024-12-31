using Volo.Abp.Modularity;

namespace Tanzeem;

[DependsOn(
    typeof(TanzeemDomainModule),
    typeof(TanzeemTestBaseModule)
)]
public class TanzeemDomainTestModule : AbpModule
{

}
