using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Tanzeem.Assignments.Dtos;
using Tanzeem.Permissions;

namespace Tanzeem.Assignments;

[Authorize(TanzeemPermissions.Assignments.Default)]
public class AssignmentAppService : TanzeemAppService, IAssignmentAppService
{
    private readonly IAssignmentRepository _assignmentRepository;

    public AssignmentAppService(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }


    [Authorize(TanzeemPermissions.Assignments.Create)]
    public async Task<AssignmentDto> CreateAsync(CreateAssignmentDto input)
    {
        var assignment = ObjectMapper.Map<CreateAssignmentDto, Assignment>(input);

        assignment = await _assignmentRepository.InsertAsync(assignment, autoSave: true);

        var dto = ObjectMapper.Map<Assignment, AssignmentDto>(assignment);

        return dto;
    }

    [Authorize(TanzeemPermissions.Assignments.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _assignmentRepository.DeleteAsync(id);
    }

    [Authorize(TanzeemPermissions.Assignments.Get)]
    public async Task<AssignmentDto> GetAsync(Guid id)
    {
        var assignment = await _assignmentRepository.GetAsync(id);

        var dto = ObjectMapper.Map<Assignment, AssignmentDto>(assignment);

        return dto;
    }

    [Authorize(TanzeemPermissions.Assignments.GetList)]
    public async Task<List<AssignmentDto>> GetListAsync(GetAssignmentListFilter filter)
    {
        var mappedFilter = ObjectMapper.Map<GetAssignmentListFilter, AssignmentListFilter>(filter);

        var assignments = await _assignmentRepository.GetListAsync(mappedFilter);

        var dtos = ObjectMapper.Map<List<Assignment>, List<AssignmentDto>>(assignments);

        return dtos;
    }

    [Authorize(TanzeemPermissions.Assignments.Update)]
    public async Task<AssignmentDto> UpdateAsync(Guid id, UpdateAssignmentDto input)
    {
        var assignment = await _assignmentRepository.GetAsync(id);

        ObjectMapper.Map(input, assignment);

        assignment = await _assignmentRepository.UpdateAsync(assignment, autoSave: true);

        var dto = ObjectMapper.Map<Assignment, AssignmentDto>(assignment);

        return dto;
    }


    [Authorize(TanzeemPermissions.Assignments.AssignUsers)]
    public async Task AssignUsersAsync(Guid assignmentId, List<Guid> userIds)
    {
        await _assignmentRepository.AssignUsersAsync(assignmentId, userIds);
    }
}