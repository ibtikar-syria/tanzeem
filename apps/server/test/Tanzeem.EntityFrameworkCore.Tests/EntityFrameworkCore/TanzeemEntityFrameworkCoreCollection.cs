using Xunit;

namespace Tanzeem.EntityFrameworkCore;

[CollectionDefinition(TanzeemTestConsts.CollectionDefinitionName)]
public class TanzeemEntityFrameworkCoreCollection : ICollectionFixture<TanzeemEntityFrameworkCoreFixture>
{

}
