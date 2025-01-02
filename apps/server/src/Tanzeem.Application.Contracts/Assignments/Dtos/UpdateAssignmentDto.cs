namespace Tanzeem.Assignments.Dtos;

public class UpdateAssignmentDto
{
    public string Title { get; set; }

    public UpdateAssignmentDto(string title)
    {
        Title = title;
    }
}