using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Tanzeem.Teams.Dtos;

public class GetTeamListFilter(List<Guid>? teamIds = null, List<Guid>? assignedUserIds = null, Guid? parentId = null, string? titleContains = null) : PagedAndSortedResultRequestDto
{
    public List<Guid>? TeamIds { get; set; } = teamIds;
    public List<Guid>? AssignedUserIds { get; set; } = assignedUserIds;
    public Guid? ParentId { get; set; } = parentId;
    public string? TitleContains { get; set; } = titleContains;
}