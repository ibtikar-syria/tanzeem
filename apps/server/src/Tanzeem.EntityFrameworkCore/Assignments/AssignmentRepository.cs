using System;
using Tanzeem.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Tanzeem.Assignments;
public class AssignmentRepository : EfCoreRepository<TanzeemDbContext, Assignment, Guid>, IAssignmentRepository
{
    public AssignmentRepository(IDbContextProvider<TanzeemDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}
