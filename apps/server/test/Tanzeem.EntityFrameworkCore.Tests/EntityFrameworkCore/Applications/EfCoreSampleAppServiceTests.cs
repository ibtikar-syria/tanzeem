using Tanzeem.Samples;
using Xunit;

namespace Tanzeem.EntityFrameworkCore.Applications;

[Collection(TanzeemTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<TanzeemEntityFrameworkCoreTestModule>
{

}
