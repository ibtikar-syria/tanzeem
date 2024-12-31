using System;
using Volo.Abp.Domain.Repositories;

namespace Tanzeem.Assignments;

public interface IAssignmentRepository : IRepository<Assignment, Guid>
{
}
