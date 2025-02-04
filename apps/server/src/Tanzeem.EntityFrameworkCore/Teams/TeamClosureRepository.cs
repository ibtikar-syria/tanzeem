using System;
using Tanzeem.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Tanzeem.Teams;

public class TeamClosureRepository(IDbContextProvider<TanzeemDbContext> dbContextProvider) : EfCoreRepository<TanzeemDbContext, TeamClosure, Guid>(dbContextProvider), ITeamClosureRepository
{
}
