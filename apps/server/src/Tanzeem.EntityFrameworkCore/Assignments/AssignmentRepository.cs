using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tanzeem.Assignments.Dtos;
using Tanzeem.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;

namespace Tanzeem.Assignments;


public static class AssignmentEfCoreQueryableExtensions
{
    public static IQueryable<Assignment> IncludeDetails(this IQueryable<Assignment> queryable)
    {
        return queryable.Include(x => x.AssignmentUsers);
    }
}

public class AssignmentRepository : EfCoreRepository<TanzeemDbContext, Assignment, Guid>, IAssignmentRepository
{
    private readonly IGuidGenerator _guidGenerator;
    public AssignmentRepository(IDbContextProvider<TanzeemDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
    {
        _guidGenerator = guidGenerator;
    }

    public override async Task<IQueryable<Assignment>> WithDetailsAsync()
    {
        var queryable = await GetQueryableAsync();
        return queryable.IncludeDetails();
    }

    public async Task AssignUsersAsync(Guid assignmentId, List<Guid> userIds)
    {
        var assignment = await GetAsync(assignmentId, includeDetails: true);

        assignment.AssignmentUsers ??= [];

        var existingUserIds = assignment.AssignmentUsers.Select(au => au.UserId).Where(userId => userIds.Contains(userId)).ToList();
        if (existingUserIds.Count > 0)
        {
            Logger.LogWarning("Some of the users are already assigned to the assignment. Skipping them...");
        }

        userIds = [.. userIds.Except(existingUserIds)];

        foreach (var userId in userIds)
        {
            assignment.AssignmentUsers.Add(new AssignmentUser(_guidGenerator.Create(), assignmentId, userId));
        }

        await UpdateAsync(assignment);
    }

    public async Task<List<Assignment>> GetListAsync(AssignmentListFilter filter)
    {
        var queryable = await GetQueryableAsync();

        var result = queryable
            .Include(x => x.AssignmentUsers)
            .WhereIf(!filter.TitleContains.IsNullOrWhiteSpace(), x => x.Title.Contains(filter.TitleContains!))
            .WhereIf(filter.AssignmentIds != null && filter.AssignmentIds?.Count > 0, x => filter.AssignmentIds!.Contains(x.Id))
            .WhereIf(filter.AssignedUserIds != null && filter.AssignedUserIds?.Count > 0, x => x.AssignmentUsers.Any(au => filter.AssignedUserIds!.Contains(au.UserId)));

        var list = await result.ToListAsync();

        return list;
    }
}
