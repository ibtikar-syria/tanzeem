using System;
using Tanzeem.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Tanzeem.Teams;

public class TeamUserClosureRepository(IDbContextProvider<TanzeemDbContext> dbContextProvider) : EfCoreRepository<TanzeemDbContext, TeamUserClosure, Guid>(dbContextProvider), ITeamUserClosureRepository
{
}
