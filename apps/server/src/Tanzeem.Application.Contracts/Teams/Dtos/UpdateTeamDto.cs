namespace Tanzeem.Teams.Dtos;

public class UpdateTeamDto(string title)
{
    public string Title { get; set; } = title;
}