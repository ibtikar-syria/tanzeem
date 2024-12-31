using Volo.Abp.Modularity;

namespace Tanzeem;

[DependsOn(
    typeof(TanzeemApplicationModule),
    typeof(TanzeemDomainTestModule)
)]
public class TanzeemApplicationTestModule : AbpModule
{

}
