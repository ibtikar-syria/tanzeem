using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Tanzeem.Teams.Dtos;
using Tanzeem.Permissions;
using Volo.Abp.Application.Services;

namespace Tanzeem.Teams;

[Authorize(TanzeemPermissions.Teams.Default)]
public class TeamAppService : ApplicationService, ITeamAppService
{
    private readonly ITeamRepository _teamRepository;

    public TeamAppService(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }


    [Authorize(TanzeemPermissions.Teams.Create)]
    public async Task<TeamDto> CreateAsync(CreateTeamDto input)
    {
        var team = ObjectMapper.Map<CreateTeamDto, Team>(input);

        team = await _teamRepository.InsertAsync(team, autoSave: true);

        var dto = ObjectMapper.Map<Team, TeamDto>(team);

        return dto;
    }

    [Authorize(TanzeemPermissions.Teams.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _teamRepository.DeleteAsync(id);
    }

    [Authorize(TanzeemPermissions.Teams.Get)]
    public async Task<TeamDto> GetAsync(Guid id)
    {
        var team = await _teamRepository.GetAsync(id);

        var dto = ObjectMapper.Map<Team, TeamDto>(team);

        return dto;
    }

    [Authorize(TanzeemPermissions.Teams.GetList)]
    public async Task<List<TeamDto>> GetListAsync(GetTeamListFilter filter)
    {
        var mappedFilter = ObjectMapper.Map<GetTeamListFilter, TeamListFilter>(filter);

        var teams = await _teamRepository.GetListAsync(mappedFilter);

        var dtos = ObjectMapper.Map<List<Team>, List<TeamDto>>(teams);

        return dtos;
    }

    [Authorize(TanzeemPermissions.Teams.Update)]
    public async Task<TeamDto> UpdateAsync(Guid id, UpdateTeamDto input)
    {
        var team = await _teamRepository.GetAsync(id);

        ObjectMapper.Map(input, team);

        team = await _teamRepository.UpdateAsync(team, autoSave: true);

        var dto = ObjectMapper.Map<Team, TeamDto>(team);

        return dto;
    }


    [Authorize(TanzeemPermissions.Teams.AssignUsers)]
    public async Task AssignUsersAsync(Guid teamId, List<Guid> userIds)
    {
        await _teamRepository.AssignUsersAsync(teamId, userIds);
    }
}