using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tanzeem.Teams;
using Tanzeem.Teams.Dtos;

namespace Tanzeem.Controllers.Teams;

[Route("api/teams")]
public class TeamController : TanzeemController, ITeamAppService
{
    private readonly ITeamAppService _teamAppService;

    public TeamController(ITeamAppService teamAppService)
    {
        _teamAppService = teamAppService;
    }

    [HttpPost]
    [Route("")]
    public Task<TeamDto> CreateAsync(CreateTeamDto input)
    {
        return _teamAppService.CreateAsync(input);
    }

    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _teamAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("{id}")]
    public Task<TeamDto> GetAsync(Guid id)
    {
        return _teamAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("")]
    public Task<List<TeamDto>> GetListAsync(GetTeamListFilter filter)
    {
        return _teamAppService.GetListAsync(filter);
    }

    [HttpPut]
    [Route("{id}")]
    public Task<TeamDto> UpdateAsync(Guid id, UpdateTeamDto input)
    {
        return _teamAppService.UpdateAsync(id, input);
    }

    [HttpPost]
    [Route("{teamId}/assign-users")]
    public Task AssignUsersAsync(Guid teamId, List<Guid> userIds)
    {
        return _teamAppService.AssignUsersAsync(teamId, userIds);
    }
}