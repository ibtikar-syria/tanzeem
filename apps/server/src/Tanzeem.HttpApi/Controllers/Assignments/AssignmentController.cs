using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tanzeem.Assignments;
using Tanzeem.Assignments.Dtos;

namespace Tanzeem.Controllers.Assignments;

[Route("api/assignments")]
public class AssignmentController : TanzeemController, IAssignmentAppService
{
    private readonly IAssignmentAppService _assignmentAppService;

    public AssignmentController(IAssignmentAppService assignmentAppService)
    {
        _assignmentAppService = assignmentAppService;
    }

    [HttpPost]
    [Route("")]
    public Task<AssignmentDto> CreateAsync(CreateAssignmentDto input)
    {
        return _assignmentAppService.CreateAsync(input);
    }

    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _assignmentAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("{id}")]
    public Task<AssignmentDto> GetAsync(Guid id)
    {
        return _assignmentAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("")]
    public Task<List<AssignmentDto>> GetListAsync(GetAssignmentListFilter filter)
    {
        return _assignmentAppService.GetListAsync(filter);
    }

    [HttpPut]
    [Route("{id}")]
    public Task<AssignmentDto> UpdateAsync(Guid id, UpdateAssignmentDto input)
    {
        return _assignmentAppService.UpdateAsync(id, input);
    }

    [HttpPost]
    [Route("{assignmentId}/assign-users")]
    public Task AssignUsersAsync(Guid assignmentId, List<Guid> userIds)
    {
        return _assignmentAppService.AssignUsersAsync(assignmentId, userIds);
    }
}