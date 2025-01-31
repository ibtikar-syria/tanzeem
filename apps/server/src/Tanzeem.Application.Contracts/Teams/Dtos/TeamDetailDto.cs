using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Tanzeem.Teams.Dtos;

public class TeamDetailDto : EntityDto<Guid>
{
    public string Title { get; set; }
    public ICollection<TeamDetailDto> Children { get; set; }

    public TeamDetailDto(Guid id, string title, ICollection<TeamDetailDto> children)
    {
        Id = id;
        Title = title;
        Children = children;
    }
}