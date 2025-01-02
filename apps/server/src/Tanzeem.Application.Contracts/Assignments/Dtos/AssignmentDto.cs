using System;
using Volo.Abp.Application.Dtos;

namespace Tanzeem.Assignments.Dtos;

public class AssignmentDto : EntityDto<Guid>
{
    public string Title { get; set; }

    public AssignmentDto(Guid id, string title)
    {
        Id = id;
        Title = title;
    }
}