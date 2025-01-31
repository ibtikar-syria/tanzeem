using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tanzeem.Assignments.Dtos;
using Volo.Abp.Application.Services;

namespace Tanzeem.Assignments;

public interface IAssignmentAppService : IApplicationService
{
    Task<List<AssignmentDto>> GetListAsync(GetAssignmentListFilter filter);
    Task<AssignmentDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<AssignmentDto> UpdateAsync(Guid id, UpdateAssignmentDto input);
    Task<AssignmentDto> CreateAsync(CreateAssignmentDto input);
    Task AssignUsersAsync(Guid assignmentId, List<Guid> userIds);
}