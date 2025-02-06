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
using Volo.Abp.Domain.Repositories;
using System.Threading;
using System.Linq.Dynamic.Core;

namespace Tanzeem.Teams;

public static class TeamEfCoreQueryableExtensions
{
    public static IQueryable<Team> IncludeDetails(this IQueryable<Team> queryable)
    {
        return queryable.Include(x => x.TeamUsers);
    }
}

public class TeamRepository(IDbContextProvider<TanzeemDbContext> dbContextProvider, IGuidGenerator guidGenerator, ITeamUserClosureRepository teamUserClosureRepository, ITeamClosureRepository teamClosureRepository) : EfCoreRepository<TanzeemDbContext, Team, Guid>(dbContextProvider), ITeamRepository
{
    private readonly IGuidGenerator _guidGenerator = guidGenerator;
    private readonly ITeamUserClosureRepository _teamUserClosureRepository = teamUserClosureRepository;
    private readonly ITeamClosureRepository _teamClosureRepository = teamClosureRepository;

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
            .WhereIf(filter.AssignedUserIds != null && filter.AssignedUserIds?.Count > 0, x => x.TeamUsers.Any(au => filter.AssignedUserIds!.Contains(au.UserId)))
            .OrderBy(filter.Sorting ?? "Title asc");

        var list = await queryable.ToListAsync();

        return list;
    }

    public async Task<TeamDetailQueryDto?> GetDetailAsync(Guid id, int depth, bool includeDetails, string? sortChildrenBy)
    {
        var queryable = await GetQueryableAsync();

        queryable = queryable
            .Include(x => x.TeamUsers);

        var team = await queryable
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (team == null)
        {
            return null;
        }

        var subTeams = await GetSubTeamsAsync(id, depth);

        // loop all sub-teams, and sort them by the given property
        if (sortChildrenBy != null)
        {
            subTeams.SortAllChildrenBy(sortChildrenBy);
        }

        return subTeams;
    }

    public async Task<Dictionary<Guid, int>> GetUserTeamIdsAsync(Guid userId, int depth)
    {
        var tuQueryable = await _teamUserClosureRepository.GetQueryableAsync();

        var teamIds = await tuQueryable
            .Where(au => au.UserId == userId)
            .Where(au => au.Depth <= depth)
            .Select(au => new { au.TeamId, au.Depth })
            .ToListAsync();

        return teamIds.ToDictionary(x => x.TeamId, x => x.Depth);
    }

    public async Task<Dictionary<Team, int>> GetUserTeamsAsync(Guid userId, int depth)
    {
        var teamIdsDict = await GetUserTeamIdsAsync(userId, depth);
        var teamIds = teamIdsDict.Keys.ToList();

        var queryable = await GetQueryableAsync();

        queryable = queryable
            .Include(x => x.TeamUsers);


        queryable = queryable
            .Where(x => teamIds.Contains(x.Id));

        var list = await queryable.ToListAsync();

        var resDict = list.ToDictionary(x => x, x => teamIdsDict[x.Id]);

        return resDict;
    }

    public async Task<List<(Guid, int)>> GetSubTeamIdsAsync(Guid teamId, int depth)
    {
        var teamClosureQueryable = await _teamClosureRepository.GetQueryableAsync();
        var subTeamIds = await teamClosureQueryable
            .Where(x => x.TeamId == teamId)
            .Where(x => x.Depth <= depth)
            .Select(x => new { x.ChildTeamId, x.Depth })
            .ToListAsync();

        var res = subTeamIds.Select(x => (x.ChildTeamId, x.Depth)).ToList();

        return res;
    }

    public async Task<TeamDetailQueryDto> GetSubTeamsAsync(Guid teamId, int depth)
    {
        var teamIdsDict = await GetSubTeamIdsAsync(teamId, depth);
        var teamIds = teamIdsDict.Select(x => x.Item1).ToList();

        var queryable = await GetQueryableAsync();

        queryable = queryable
            .Include(x => x.TeamUsers);

        queryable = queryable
            .Where(x => teamIds.Contains(x.Id));

        var list = await queryable.ToListAsync();

        var resDict = list.ToDictionary(x => x, x => teamIdsDict.First(y => y.Item1 == x.Id).Item2);

        var team = TeamDetailQueryDto.FromDictionary(resDict);

        return team;
    }

    public override async Task<Team> InsertAsync(Team entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var res = await base.InsertAsync(entity, autoSave, cancellationToken);

        var userClosures = GetUserClosuresFor(entity);
        await _teamUserClosureRepository.InsertManyAsync(userClosures, autoSave, cancellationToken);

        var teamClosures = GetTeamClosuresFor(entity);
        await _teamClosureRepository.InsertManyAsync(teamClosures, autoSave, cancellationToken);

        return res;
    }

    public override async Task InsertManyAsync(IEnumerable<Team> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var teams = entities.ToList();

        var userClosures = new List<TeamUserClosure>();
        var teamClosures = new List<TeamClosure>();

        foreach (var team in teams)
        {
            userClosures.AddRange(GetUserClosuresFor(team));
            teamClosures.AddRange(GetTeamClosuresFor(team));
        }

        await _teamUserClosureRepository.InsertManyAsync(userClosures, autoSave, cancellationToken);
        await _teamClosureRepository.InsertManyAsync(teamClosures, autoSave, cancellationToken);

        await base.InsertManyAsync(teams, autoSave, cancellationToken);
    }

    private List<TeamClosure> GetTeamClosuresFor(Team team)
    {
        var closures = new List<TeamClosure>();

        var parentClosure = new TeamClosure(_guidGenerator.Create(), team.Id, team.Id, 0);
        closures.Add(parentClosure);

        foreach (var child in team.Children)
        {
            var subClosures = GetTeamClosuresFor(child);
            closures.AddRange(subClosures);

            var parentSubClosures = subClosures
            // only the subitems that are already increased by the child, without the ones that belong to their subchildren, which are NOT one level below `team`
            .Where(x => x.TeamId == child.Id).Select(x => new TeamClosure(_guidGenerator.Create(), team.Id, x.ChildTeamId, x.Depth + 1)).ToList();
            closures.AddRange(parentSubClosures);
        }

        return closures;
    }

    private List<TeamUserClosure> GetUserClosuresFor(Team team)
    {
        var closures = new List<TeamUserClosure>();

        foreach (var teamUser in team.TeamUsers)
        {
            var closure = new TeamUserClosure(_guidGenerator.Create(), team.Id, teamUser.UserId, 0);
            closures.Add(closure);
        }

        foreach (var child in team.Children)
        {
            var childClosures = GetUserClosuresFor(child);
            closures.AddRange(childClosures);

            var parentChildClosures = childClosures.Select(x => new TeamUserClosure(_guidGenerator.Create(), team.Id, x.UserId, x.Depth + 1));
            closures.AddRange(parentChildClosures);
        }

        return closures;
    }
}
