using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Dynamic.Core;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Tanzeem.Teams.Dtos;

public class TeamDetailQueryDto : EntityEto<Guid>
{
    public required virtual Guid? ParentId { get; set; }
    public required virtual int Depth { get; set; }
    public required virtual string Title { get; set; }
    public required virtual ICollection<TeamDetailQueryDto> Children { get; set; } = [];

    [SetsRequiredMembers]
    public TeamDetailQueryDto(Guid id, Guid? parentId, int depth, string title, List<TeamDetailQueryDto> children)
    {
        Id = id;
        ParentId = parentId;
        Depth = depth;
        Title = title;
        Children = children;
    }

    public static TeamDetailQueryDto FromDictionary(Dictionary<Team, int> teams)
    {
        var result = new List<TeamDetailQueryDto>();

        foreach (var team in teams)
        {
            var (teamEntity, depth) = team;
            var children = new List<TeamDetailQueryDto>();

            foreach (var child in teamEntity.Children)
            {
                children.Add(FromEntity(child, depth + 1));
            }

            result.Add(new TeamDetailQueryDto(teamEntity.Id, teamEntity.ParentId, depth, teamEntity.Title, children));
        }

        return new TeamDetailQueryDto(Guid.Empty, null, 0, "Root", result);
    }

    public static TeamDetailQueryDto FromEntity(Team team, int depth)
    {
        var children = new List<TeamDetailQueryDto>();

        foreach (var child in team.Children)
        {
            children.Add(FromEntity(child, depth + 1));
        }

        return new TeamDetailQueryDto(team.Id, team.ParentId, depth, team.Title, children);
    }

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
}