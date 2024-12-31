using Tanzeem.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Tanzeem.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(TanzeemEntityFrameworkCoreModule),
    typeof(TanzeemApplicationContractsModule)
)]
public class TanzeemDbMigratorModule : AbpModule
{
}
