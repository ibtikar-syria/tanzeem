using System;
using Volo.Abp.Application.Dtos;

namespace Tanzeem.Teams.Dtos;

public class TeamDto : EntityDto<Guid>
{
    public string Title { get; set; }

    public TeamDto(Guid id, string title)
    {
        Id = id;
        Title = title;
    }
}