using System;
using System.Collections.Generic;

namespace Tanzeem.Assignments.Dtos;

public class AssignmentListFilter(int maxResultCount, int skipCount, string? sorting, List<Guid>? assignmentIds = null, List<Guid>? assignedUserIds = null, string? titleContains = null)
{
    public int MaxResultCount { get; set; } = maxResultCount;
    public int SkipCount { get; set; } = skipCount;
    public string? Sorting { get; set; } = sorting;
    public List<Guid>? AssignmentIds { get; set; } = assignmentIds;
    public List<Guid>? AssignedUserIds { get; set; } = assignedUserIds;
    public string? TitleContains { get; set; } = titleContains;
}