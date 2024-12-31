using Tanzeem.Samples;
using Xunit;

namespace Tanzeem.EntityFrameworkCore.Domains;

[Collection(TanzeemTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<TanzeemEntityFrameworkCoreTestModule>
{

}
