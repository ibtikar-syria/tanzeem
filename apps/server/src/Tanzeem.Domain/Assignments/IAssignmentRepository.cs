using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tanzeem.Assignments.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Tanzeem.Assignments;

public interface IAssignmentRepository : IRepository<Assignment, Guid>
{
    public Task<List<Assignment>> GetListAsync(AssignmentListFilter filter);
    public Task AssignUsersAsync(Guid assignmentId, List<Guid> userIds);
}
