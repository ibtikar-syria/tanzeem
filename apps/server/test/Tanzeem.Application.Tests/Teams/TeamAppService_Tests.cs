using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tanzeem.Teams.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Xunit;

namespace Tanzeem.Teams;

public abstract class TeamAppService_Tests<TStartupModule> : TanzeemApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly ITeamRepository _teamRepository;
    private readonly IIdentityUserRepository _userRepository;

    public TeamAppService_Tests()
    {
        _teamRepository = GetRequiredService<ITeamRepository>();
        _userRepository = GetRequiredService<IIdentityUserRepository>();
    }

    private async Task<List<Team>> SeedTeams()
    {
        var teams = new List<Team>
        {
            new(id: Guid.NewGuid(), title: "Team 1"),
            new(id: Guid.NewGuid(), title: "Team 2"),
            new(id: Guid.NewGuid(), title: "Team 3")
        };

        foreach (var team in teams)
        {
            await _teamRepository.InsertAsync(team);
        }

        return teams;
    }

    private async Task<List<IdentityUser>> SeedUsers()
    {
        var users = new List<IdentityUser>
        {
            new(id: Guid.NewGuid(), userName: "user1", email: "adnan@adnan.com"),
            new(id: Guid.NewGuid(), userName: "user2", email: "ahmad@ahmad.com"),
        };

        await _userRepository.InsertManyAsync(users);

        return users;
    }

    private async Task<List<Team>> SeedTeamsWithUsers()
    {
        var teams = await SeedTeams();

        foreach (var team in teams)
        {
            var users = await SeedUsers();

            var userIds = users.Select(u => u.Id).ToList();

            await _teamRepository.AssignUsersAsync(team.Id, userIds);
        }

        teams = await _teamRepository.GetListAsync(includeDetails: true);

        return teams;
    }

    [Fact]
    public async Task Should_Create_Teams()
    {
        var id = Guid.NewGuid();
        var title = "Team 1";
        var team = new Team(id, title);

        await _teamRepository.InsertAsync(team);

        var foundTeam = await _teamRepository.FindAsync(id);

        Assert.NotNull(foundTeam);
        Assert.Equal(title, foundTeam.Title);
        Assert.Equal(id, foundTeam.Id);
    }

    [Fact]
    public async Task Should_Assign_Users_To_Team()
    {
        var teams = await SeedTeams();

        var first = teams[0];

        var users = await SeedUsers();

        var userIds = users.Select(u => u.Id).ToList();

        var service = GetRequiredService<ITeamAppService>();

        await service.AssignUsersAsync(first.Id, userIds);

        var foundTeam = await _teamRepository.FindAsync(first.Id, includeDetails: true);

        Assert.NotNull(foundTeam);
        Assert.Equal(2, foundTeam.TeamUsers.Count);
        Assert.Contains(foundTeam.TeamUsers, au => userIds.Contains(au.UserId));
    }

    [Fact]
    public async Task Should_Get_All_Teams()
    {
        await SeedTeams();

        var service = GetRequiredService<ITeamAppService>();
        var result = await service.GetListAsync(new());

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task Should_Filter_Teams_By_Title()
    {
        await SeedTeams();

        var service = GetRequiredService<ITeamAppService>();
        var result = await service.GetListAsync(new() { TitleContains = "Team 1" });

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Team 1", result[0].Title);
    }

    [Fact]
    public async Task Should_Filter_Teams_By_Id()
    {
        var teams = await SeedTeams();

        var first = teams[0];

        var service = GetRequiredService<ITeamAppService>();
        var result = await service.GetListAsync(new() { TeamIds = [first.Id] });

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(first.Id, result[0].Id);
    }

    [Fact]
    public async Task Should_Filter_Teams_By_UserId()
    {
        var teams = await SeedTeamsWithUsers();

        var first = teams[0];

        var second = teams[1];
        var secondUserIds = second!.TeamUsers.Select(au => au.UserId).ToList();

        var third = teams[2];
        var thirdUserIds = third!.TeamUsers.Select(au => au.UserId).ToList();

        var service = GetRequiredService<ITeamAppService>();
        var result = await service.GetListAsync(new()
        {
            AssignedUserIds = [
                secondUserIds[0],
                thirdUserIds[1]
            ]
        });

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, a => a.Id == first.Id);
        Assert.Contains(result, a => a.Id == second.Id);
        Assert.Contains(result, a => a.Id == third.Id);
    }

    [Fact]
    public async Task Should_Filter_Teams_By_Assigned_Users()
    {
        var teams = await SeedTeams();

        var first = teams[0];

        var service = GetRequiredService<ITeamAppService>();
        var result = await service.GetListAsync(new() { TeamIds = [first.Id] });

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(first.Id, result[0].Id);
    }

    [Fact]
    public async Task Should_Delete_Team()
    {
        var teams = await SeedTeams();

        var first = teams[0];

        var service = GetRequiredService<ITeamAppService>();

        await service.DeleteAsync(first.Id);

        var found = await _teamRepository.FindAsync(first.Id);

        Assert.Null(found);
    }

    [Fact]
    public async Task Should_Update_Team()
    {
        var teams = await SeedTeams();

        var first = teams[0];

        var service = GetRequiredService<ITeamAppService>();

        var updatedTitle = "Team 1 Updated";

        var input = new UpdateTeamDto(title: updatedTitle);

        var updated = await service.UpdateAsync(first.Id, input);

        Assert.NotNull(updated);
        Assert.Equal(updatedTitle, updated.Title);
    }

    [Fact]
    public async Task Should_Get_Team()
    {
        var teams = await SeedTeams();

        var first = teams[0];

        var service = GetRequiredService<ITeamAppService>();

        var found = await service.GetAsync(first.Id);

        Assert.NotNull(found);
        Assert.Equal(first.Title, found.Title);
    }
}