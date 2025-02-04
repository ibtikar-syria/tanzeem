using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Application.Dtos;

namespace Tanzeem.Teams.Dtos;

public class TeamDetailDto : EntityDto<Guid>
{
    [Required]
    public required Guid? ParentId { get; set; }
    [Required]
    public required int Depth { get; set; }
    [Required]
    public required string Title { get; set; }
    [Required]
    public required ICollection<TeamDetailDto> Children { get; set; }

    [SetsRequiredMembers]
    public TeamDetailDto(Guid? parentId, int depth, Guid id, string title, ICollection<TeamDetailDto> children)
    {
        ParentId = parentId;
        Depth = depth;
        Id = id;
        Title = title;
        Children = children;
    }
}