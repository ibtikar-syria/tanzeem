using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;
using System.Linq.Dynamic.Core;
using Volo.Abp.MultiTenancy;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Identity;

namespace Tanzeem.Teams;

public class Team : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    public virtual Guid? ParentId { get; set; }
    public virtual Team? Parent { get; set; }

    public required virtual string Title { get; set; }

    public required virtual ICollection<Team> Children { get; set; } = [];
    public required virtual ICollection<TeamUser> TeamUsers { get; set; } = [];

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

    public void AddTeam(Team team)
    {
        Children ??= [];

        if (Children.Any(t => t.Id == team.Id))
        {
            return;
        }

        Children.Add(team);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sortChildrenBy">
    /// ef does not order by default when inserting children.
    /// example:
    /// - "Title asc, CreationTime desc"
    /// - "Title desc"
    /// - "Id asc, Title desc"
    /// </param>
    public void SortAllChildrenBy(string sortChildrenBy)
    {
        if (Children == null)
        {
            return;
        }

        var queryable = Children.AsQueryable();
        Children = [.. queryable.OrderBy(sortChildrenBy)];

        foreach (var child in Children)
        {
            child.SortAllChildrenBy(sortChildrenBy);
        }
    }

    protected Team()
    {
    }

    [SetsRequiredMembers]
    public Team(Guid id, string title)
    {
        Id = id;
        Title = title;
    }

    [SetsRequiredMembers]
    public Team(Guid id, string title, List<Team> children)
    {
        Id = id;
        Title = title;
        Children = children;
    }
}