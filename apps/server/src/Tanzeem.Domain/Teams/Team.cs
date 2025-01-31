using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Tanzeem.Teams;

public class Team : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    public virtual Guid? ParentId { get; set; }
    public virtual Team Parent { get; set; }

    public virtual string Title { get; set; }

    public virtual ICollection<TeamUser> TeamUsers { get; set; }

    /// <summary>
    /// Assigns users to the Team.
    /// </summary>
    /// <param name="ids">A dictionary where the key is the relation entity id and the value is the user id.</param>
    public void AssignUsers(Dictionary<Guid, Guid> ids)
    {
        TeamUsers ??= [];

        foreach (var record in ids)
        {
            var (relationId, userId) = record;
            if (TeamUsers.Any(au => au.UserId == userId))
            {
                continue;
            }

            TeamUsers.Add(new TeamUser(relationId, Id, userId));
        }
    }

    protected Team()
    {
    }

    public Team(Guid id, string title)
    {
        Id = id;
        Title = title;
    }
}