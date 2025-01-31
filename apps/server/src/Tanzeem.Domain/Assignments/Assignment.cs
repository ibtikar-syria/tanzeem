using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Tanzeem.Assignments;

public class Assignment : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }
    public virtual string Title { get; set; }
    public virtual ICollection<AssignmentUser> AssignmentUsers { get; set; }

    /// <summary>
    /// Assigns users to the assignment.
    /// </summary>
    /// <param name="ids">A dictionary where the key is the relation entity id and the value is the user id.</param>
    public void AssignUsers(Dictionary<Guid, Guid> ids)
    {
        AssignmentUsers ??= [];

        foreach (var record in ids)
        {
            var (relationId, userId) = record;
            if (AssignmentUsers.Any(au => au.UserId == userId))
            {
                continue;
            }

            AssignmentUsers.Add(new AssignmentUser(relationId, Id, userId));
        }
    }

    protected Assignment()
    {
    }

    public Assignment(Guid id, string title)
    {
        Id = id;
        Title = title;
    }
}