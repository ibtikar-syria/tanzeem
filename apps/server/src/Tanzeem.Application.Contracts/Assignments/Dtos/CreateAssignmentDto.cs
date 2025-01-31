namespace Tanzeem.Assignments.Dtos;

public class CreateAssignmentDto(string title)
{
    public string Title { get; set; } = title;
}