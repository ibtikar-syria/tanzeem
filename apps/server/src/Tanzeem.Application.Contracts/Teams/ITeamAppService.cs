using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tanzeem.Teams.Dtos;
using Volo.Abp.Application.Services;

namespace Tanzeem.Teams;

public interface ITeamAppService : IApplicationService
{
    Task<List<TeamDto>> GetListAsync(GetTeamListFilter filter);
    Task<TeamDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<TeamDto> UpdateAsync(Guid id, UpdateTeamDto input);
    Task<TeamDto> CreateAsync(CreateTeamDto input);
    Task AssignUsersAsync(Guid teamId, List<Guid> userIds);
    public Task<TeamDetailDto> GetDetailAsync(Guid id, int depth = 0, bool includeDetails = true, string? sortChildrenBy = null);
}