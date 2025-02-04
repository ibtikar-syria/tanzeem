using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tanzeem.Teams.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Tanzeem.Teams;

public interface ITeamRepository : IRepository<Team, Guid>
{
    public Task<List<Team>> GetListAsync(TeamListFilter filter);
    public Task AssignUsersAsync(Guid teamId, List<Guid> userIds);
    public Task<TeamDetailQueryDto?> GetDetailAsync(Guid id, int depth, bool includeDetails, string? sortChildrenBy);
    public Task<Dictionary<Guid, int>> GetUserTeamIdsAsync(Guid userId, int depth);
    public Task<Dictionary<Team, int>> GetUserTeamsAsync(Guid userId, int depth);
}
