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

    public async Task<List<AssignmentDto>> GetListAsync()
    {
        var assignments = await _assignmentRepository.GetListAsync();

        var dtos = ObjectMapper.Map<List<Assignment>, List<AssignmentDto>>(assignments);

        return dtos;
    }
}