using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;
using System.Linq.Dynamic.Core;
using Volo.Abp.MultiTenancy;

namespace Tanzeem.Teams;

public class Team : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    public virtual Guid? ParentId { get; set; }
    public virtual Team Parent { get; set; }
    public virtual ICollection<Team> Children { get; set; }

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

        Children = [.. Children.AsQueryable().OrderBy(sortChildrenBy)];

        foreach (var child in Children)
        {
            child.SortAllChildrenBy(sortChildrenBy);
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