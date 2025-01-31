using System;
using System.Collections.Generic;

namespace Tanzeem.Teams.Dtos;

public class TeamListFilter(int maxResultCount, int skipCount, string? sorting, List<Guid>? teamIds = null, Guid? parentId = null, List<Guid>? assignedUserIds = null, string? titleContains = null)
{
    public int MaxResultCount { get; set; } = maxResultCount;
    public int SkipCount { get; set; } = skipCount;
    public string? Sorting { get; set; } = sorting;
    public List<Guid>? TeamIds { get; set; } = teamIds;
    // when the parent team is provided, then provide all 
    // the children of the parent team. more depth will be
    // provided in a specific endpoint.
    public Guid? ParentId { get; set; } = parentId;
    public List<Guid>? AssignedUserIds { get; set; } = assignedUserIds;
    public string? TitleContains { get; set; } = titleContains;
}