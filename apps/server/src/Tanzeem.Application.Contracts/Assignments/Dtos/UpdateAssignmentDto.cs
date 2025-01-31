namespace Tanzeem.Assignments.Dtos;

public class UpdateAssignmentDto(string title)
{
    public string Title { get; set; } = title;
}