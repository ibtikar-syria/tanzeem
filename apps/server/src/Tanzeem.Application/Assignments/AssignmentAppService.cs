using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tanzeem.Assignments.Dtos;
using Volo.Abp.Application.Services;

namespace Tanzeem.Assignments;

public class AssignmentAppService : ApplicationService, IAssignmentAppService
{
    private readonly IAssignmentRepository _assignmentRepository;

    public AssignmentAppService(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<AssignmentDto> CreateAsync(CreateAssignmentDto input)
    {
        var assignment = ObjectMapper.Map<CreateAssignmentDto, Assignment>(input);

        assignment = await _assignmentRepository.InsertAsync(assignment, autoSave: true);

        var dto = ObjectMapper.Map<Assignment, AssignmentDto>(assignment);

        return dto;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _assignmentRepository.DeleteAsync(id);
    }

    public async Task<AssignmentDto> GetAsync(Guid id)
    {
        var assignment = await _assignmentRepository.GetAsync(id);

        var dto = ObjectMapper.Map<Assignment, AssignmentDto>(assignment);

        return dto;
    }

    public async Task<List<AssignmentDto>> GetListAsync()
    {
        var assignments = await _assignmentRepository.GetListAsync();

        var dtos = ObjectMapper.Map<List<Assignment>, List<AssignmentDto>>(assignments);

        return dtos;
    }

    public async Task<AssignmentDto> UpdateAsync(Guid id, UpdateAssignmentDto input)
    {
        var assignment = await _assignmentRepository.GetAsync(id);

        ObjectMapper.Map(input, assignment);

        assignment = await _assignmentRepository.UpdateAsync(assignment, autoSave: true);

        var dto = ObjectMapper.Map<Assignment, AssignmentDto>(assignment);

        return dto;
    }
}