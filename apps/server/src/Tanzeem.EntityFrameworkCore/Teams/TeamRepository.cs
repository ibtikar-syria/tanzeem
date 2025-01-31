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
using Microsoft.EntityFrameworkCore.Query;
using Volo.Abp.Domain.Entities;

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

        queryable = queryable
            .Include(x => x.TeamUsers);

        queryable = queryable
            .WhereIf(!filter.TitleContains.IsNullOrWhiteSpace(), x => x.Title.Contains(filter.TitleContains!))
            .WhereIf(filter.TeamIds != null && filter.TeamIds?.Count > 0, x => filter.TeamIds!.Contains(x.Id))
            .WhereIf(filter.ParentId != null, x => x.ParentId == filter.ParentId)
            .WhereIf(filter.AssignedUserIds != null && filter.AssignedUserIds?.Count > 0, x => x.TeamUsers.Any(au => filter.AssignedUserIds!.Contains(au.UserId)));

        var list = await queryable.ToListAsync();

        return list;
    }

    public async Task<Team?> GetDetailAsync(Guid id, int depth, bool includeDetails, string? sortChildrenBy)
    {
        var queryable = await GetQueryableAsync();

        queryable = queryable
            .Include(x => x.TeamUsers);

        Team? team;

        if (depth == 1)
        {
            queryable = queryable.Include(x => x.Children);

            team = await queryable.FirstOrDefaultAsync(x => x.Id == id);
        }
        else if (depth == 2)
        {
            var includedQueryable = queryable.Include(x => x.Children).ThenInclude(
                    x => x.Children
                );

            team = await includedQueryable.FirstOrDefaultAsync(x => x.Id == id);
        }
        else
        {
            // queryable.Include returns a different type than includedQueryable.ThenInclude,
            // so we assign it and then start the loop from 2
            var includedQueryable = queryable.Include(x => x.Children).ThenInclude(
                x => x.Children
            );
            for (var i = 2; i < depth; i++)
            {
                includedQueryable = includedQueryable.ThenInclude(
                   x => x.Children
               );
            }

            team = await includedQueryable.FirstOrDefaultAsync(x => x.Id == id);
        }

        if (team == null)
        {
            throw new EntityNotFoundException(typeof(Team), id);
        }

        // loop all sub-teams, and sort them by the given property
        if (sortChildrenBy != null)
        {
            team.SortAllChildrenBy(sortChildrenBy);
        }

        return team;
    }
}
