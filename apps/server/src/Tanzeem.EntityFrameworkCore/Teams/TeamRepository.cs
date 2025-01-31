using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tanzeem.Teams.Dtos;
using Tanzeem.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;

namespace Tanzeem.Teams;

public static class TeamEfCoreQueryableExtensions
{
    public static IQueryable<Team> IncludeDetails(this IQueryable<Team> queryable)
    {
        return queryable.Include(x => x.TeamUsers);
    }
}

public class TeamRepository : EfCoreRepository<TanzeemDbContext, Team, Guid>, ITeamRepository
{
    private readonly IGuidGenerator _guidGenerator;
    public TeamRepository(IDbContextProvider<TanzeemDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
    {
        _guidGenerator = guidGenerator;
    }

    public override async Task<IQueryable<Team>> WithDetailsAsync()
    {
        var queryable = await GetQueryableAsync();
        return queryable.IncludeDetails();
    }

    public async Task AssignUsersAsync(Guid teamId, List<Guid> userIds)
    {
        var team = await GetAsync(teamId, includeDetails: true);

        team.TeamUsers ??= [];

        var existingUserIds = team.TeamUsers.Select(au => au.UserId).Where(userId => userIds.Contains(userId)).ToList();
        if (existingUserIds.Count > 0)
        {
            Logger.LogWarning("Some of the users are already assigned to the team. Skipping them...");
        }

        userIds = [.. userIds.Except(existingUserIds)];

        foreach (var userId in userIds)
        {
            team.TeamUsers.Add(new TeamUser(_guidGenerator.Create(), teamId, userId));
        }

        await UpdateAsync(team);
    }

    public async Task<List<Team>> GetListAsync(TeamListFilter filter)
    {
        var queryable = await GetQueryableAsync();

        var result = queryable
            .Include(x => x.TeamUsers)
            .WhereIf(!filter.TitleContains.IsNullOrWhiteSpace(), x => x.Title.Contains(filter.TitleContains!))
            .WhereIf(filter.TeamIds != null && filter.TeamIds?.Count > 0, x => filter.TeamIds!.Contains(x.Id))
            .WhereIf(filter.ParentId != null, x => x.ParentId == filter.ParentId)
            .WhereIf(filter.AssignedUserIds != null && filter.AssignedUserIds?.Count > 0, x => x.TeamUsers.Any(au => filter.AssignedUserIds!.Contains(au.UserId)));

        var list = await result.ToListAsync();

        return list;
    }
}
