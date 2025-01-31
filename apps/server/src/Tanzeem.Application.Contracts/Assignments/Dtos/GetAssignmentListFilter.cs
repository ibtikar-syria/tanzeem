using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Tanzeem.Assignments.Dtos;

public class GetAssignmentListFilter(List<Guid>? assignmentIds = null, List<Guid>? assignedUserIds = null, string? titleContains = null) : PagedAndSortedResultRequestDto
{
    public List<Guid>? AssignmentIds { get; set; } = assignmentIds;
    public List<Guid>? AssignedUserIds { get; set; } = assignedUserIds;
    public string? TitleContains { get; set; } = titleContains;
}