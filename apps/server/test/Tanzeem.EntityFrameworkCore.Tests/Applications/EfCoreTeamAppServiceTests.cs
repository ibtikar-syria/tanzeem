using Tanzeem.Teams;
using Tanzeem.EntityFrameworkCore;

namespace Tanzeem.Applications;

public class EfCoreTeamAppServiceTests : TeamAppService_Tests<TanzeemEntityFrameworkCoreTestModule>
{
    public EfCoreTeamAppServiceTests()
    {
    }
}
