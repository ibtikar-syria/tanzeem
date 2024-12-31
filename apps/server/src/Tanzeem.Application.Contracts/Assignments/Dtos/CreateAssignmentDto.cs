using System;
using Volo.Abp.Application.Dtos;

namespace Tanzeem.Assignments.Dtos;

public class CreateAssignmentDto
{
    public string Title { get; set; }

    public CreateAssignmentDto(string title)
    {
        Title = title;
    }
}