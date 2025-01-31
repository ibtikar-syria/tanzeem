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
    public Task<Team?> GetDetailAsync(Guid id, int depth, bool includeDetails, string? sortChildrenBy);
}
