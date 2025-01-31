namespace Tanzeem.Teams.Dtos;

public class CreateTeamDto(string title)
{
    public string Title { get; set; } = title;
}