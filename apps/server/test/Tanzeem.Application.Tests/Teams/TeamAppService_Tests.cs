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
            new(id: Guid.NewGuid(), title: "Team 1", children: [
                new(id: Guid.NewGuid(), title: "Team 1.1", children:[
                    new(id: Guid.NewGuid(), title: "Team 1.1.1"),
                    new(id: Guid.NewGuid(), title: "Team 1.1.2"),
                ]),
                new(id: Guid.NewGuid(), title: "Team 1.2")
            ]),
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
    public async Task Should_Assign_Sub_Teams_To_User()
    {
        var users = await SeedUsers();

        var tid1 = Guid.NewGuid();
        var tid2 = Guid.NewGuid();
        var tid3 = Guid.NewGuid();
        var tid4 = Guid.NewGuid();
        var tid5 = Guid.NewGuid();

        var team = new Team(tid1, "Team 1", children: [
            new(id: tid2, title: "Team 1.1", children: [
                new(id: tid3, title: "Team 1.1.1"),
                new(id: tid4, title: "Team 1.1.2"),
            ]),
            new(id: tid5, title: "Team 1.2")
        ]);

        await _teamRepository.InsertAsync(team);

        var userIds = users.Select(u => u.Id).ToList();

        await _teamRepository.AssignUsersAsync(team.Id, userIds);

        var teamIds = await _teamRepository.GetUserTeamIdsAsync(userIds[0], depth: 3);
        var teams = await _teamRepository.GetUserTeamsAsync(userIds[0], depth: 3);

        Assert.NotNull(teamIds);
        Assert.Contains(team.Id, teamIds);
        Assert.Contains(tid1, teamIds);
        Assert.Contains(tid2, teamIds);
        Assert.Contains(tid3, teamIds);
        Assert.Contains(tid4, teamIds);
        Assert.Contains(tid5, teamIds);

        Assert.NotNull(teams);
        Assert.Contains(team, teams);
        Assert.Contains(team.Children.ElementAt(0), teams);
        Assert.Contains(team.Children.ElementAt(0).Children.ElementAt(0), teams);
        Assert.Contains(team.Children.ElementAt(0).Children.ElementAt(1), teams);
        Assert.Contains(team.Children.ElementAt(1), teams);
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
        Assert.Equal(7, result.Count);
    }

    [Fact]
    public async Task Should_Filter_Teams_By_Title()
    {
        await SeedTeams();

        var service = GetRequiredService<ITeamAppService>();
        var result = await service.GetListAsync(new() { TitleContains = "Team 1", Sorting = "Title asc" });

        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
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

    private async Task<List<Team>> SeedTeamsWithHierarchy()
    {
        var teams = new List<Team>
        {
            new(id: Guid.NewGuid(), title: "Team 1"),
            new(id: Guid.NewGuid(), title: "Team 2")
        };

        teams[0].AddTeam(new(id: Guid.NewGuid(), title: "Team 1.1"));
        teams[0].AddTeam(new(id: Guid.NewGuid(), title: "Team 1.2"));

        teams[0].Children.ElementAt(0).AddTeam(new(id: Guid.NewGuid(), title: "Team 1.1.1"));

        teams[0].Children.ElementAt(0).Children.ElementAt(0).AddTeam(new(id: Guid.NewGuid(), title: "Team 1.1.1.1"));
        teams[0].Children.ElementAt(0).Children.ElementAt(0).AddTeam(new(id: Guid.NewGuid(), title: "Team 1.1.1.2"));

        teams[0].Children.ElementAt(0).Children.ElementAt(0).Children.ElementAt(1).AddTeam(new(id: Guid.NewGuid(), title: "Team 1.1.1.2.1"));
        teams[0].Children.ElementAt(0).Children.ElementAt(0).Children.ElementAt(1).AddTeam(new(id: Guid.NewGuid(), title: "Team 1.1.1.2.2"));

        await _teamRepository.InsertManyAsync(teams);

        return teams;
    }

    [Fact]
    public async Task Should_Get_Team_With_Depth_1()
    {
        var teams = await SeedTeamsWithHierarchy();

        var service = GetRequiredService<ITeamAppService>();

        var result = await service.GetDetailAsync(teams[0].Id, depth: 1, sortChildrenBy: "Title asc");

        Assert.NotNull(result);
        // itself, 1.1, 1.2
        Assert.Equal(3, result.Children.Count);
        Assert.Equal("Team 1", result.Children.ElementAt(0).Title);
        Assert.Equal("Team 1.1", result.Children.ElementAt(1).Title);
        Assert.Equal("Team 1.2", result.Children.ElementAt(2).Title);
    }

    [Fact]
    public async Task Should_Get_Team_With_Depth_2()
    {
        var teams = await SeedTeamsWithHierarchy();

        var service = GetRequiredService<ITeamAppService>();

        var result = await service.GetDetailAsync(teams[0].Id, depth: 2, sortChildrenBy: "Title asc");

        var firstChild = result.Children.ElementAt(0);
        var secondChild = result.Children.ElementAt(1);
        var thirdChild = result.Children.ElementAt(2);

        Assert.NotNull(result);
        Assert.Equal(3, result.Children.Count);
        Assert.Equal("Team 1", firstChild.Title);
        Assert.Equal("Team 1.1", secondChild.Title);
        Assert.Equal("Team 1.2", thirdChild.Title);
    }

    [Fact]
    public async Task Should_Get_Team_With_Depth_3()
    {
        var teams = await SeedTeamsWithHierarchy();

        var service = GetRequiredService<ITeamAppService>();

        var result = await service.GetDetailAsync(teams[0].Id, depth: 3, sortChildrenBy: "Title asc");

        var firstChild = result.Children.ElementAt(0);
        var first2ndChild = firstChild.Children.ElementAt(0);
        var first3rdChild = first2ndChild.Children.ElementAt(0);
        var second3rdChild = first2ndChild.Children.ElementAt(1);

        Assert.NotNull(result);
        Assert.NotNull(firstChild);
        Assert.NotNull(first2ndChild);
        Assert.NotNull(first3rdChild);
        Assert.NotNull(second3rdChild);

        Assert.Equal(2, result.Children.Count);
        Assert.Equal("Team 1.1", firstChild.Title);

        Assert.Single(firstChild.Children);
        Assert.Equal("Team 1.1.1", first2ndChild.Title);

        Assert.Equal(2, first2ndChild.Children.Count);
        Assert.Equal("Team 1.1.1.1", first3rdChild.Title);
        Assert.Equal("Team 1.1.1.2", second3rdChild.Title);
    }

    [Fact]
    public async Task Should_Get_Team_With_Depth_4()
    {
        var teams = await SeedTeamsWithHierarchy();

        var service = GetRequiredService<ITeamAppService>();

        var result = await service.GetDetailAsync(teams[0].Id, depth: 4, sortChildrenBy: "Title asc");

        var secondChild = result.Children.ElementAt(1);
        var second2ndChild = secondChild.Children.ElementAt(0);
        var first3rdChild = second2ndChild.Children.ElementAt(0);
        var third3rdChild = second2ndChild.Children.ElementAt(1);
        var first4thChild = third3rdChild.Children.ElementAt(0);
        var third4thChild = third3rdChild.Children.ElementAt(1);

        Assert.NotNull(result);
        Assert.NotNull(secondChild);
        Assert.NotNull(second2ndChild);
        Assert.NotNull(first3rdChild);
        Assert.NotNull(third3rdChild);
        Assert.NotNull(first4thChild);
        Assert.NotNull(third4thChild);

        Assert.Equal(2, result.Children.Count);
        Assert.Equal("Team 1.1", secondChild.Title);

        Assert.Single(secondChild.Children);
        Assert.Equal("Team 1.1.1", second2ndChild.Title);

        Assert.Equal(2, second2ndChild.Children.Count);
        Assert.Equal("Team 1.1.1.1", first3rdChild.Title);
        Assert.Equal("Team 1.1.1.2", third3rdChild.Title);

        Assert.Empty(first3rdChild.Children);

        Assert.Equal(2, third3rdChild.Children.Count);
        Assert.Equal("Team 1.1.1.2.1", first4thChild.Title);
        Assert.Equal("Team 1.1.1.2.2", third4thChild.Title);
    }

    [Fact]
    public async Task Should_Get_Team_With_Sorted_Children()
    {
        // Arrange
        var teams = new List<Team>
        {
            new(id: Guid.NewGuid(), title: "Team 1"),
            new(id: Guid.NewGuid(), title: "Team 2")
        };

        teams[0].AddTeam(new(id: Guid.NewGuid(), title: "Team 1.1"));
        teams[0].Children.ElementAt(0).CreationTime = DateTime.Now;
        teams[0].AddTeam(new(id: Guid.NewGuid(), title: "Team 1.2"));
        teams[0].Children.ElementAt(1).CreationTime = DateTime.Now.AddDays(-1);


        teams[0].Children.ElementAt(0).AddTeam(new(id: Guid.NewGuid(), title: "Team 1.1.1"));


        teams[0].Children.ElementAt(0).Children.ElementAt(0).AddTeam(new(id: Guid.NewGuid(), title: "Team 1.1.1.1")); teams[0].Children.ElementAt(0).Children.ElementAt(0).Children.ElementAt(0).CreationTime = DateTime.Now;

        teams[0].Children.ElementAt(0).Children.ElementAt(0).AddTeam(new(id: Guid.NewGuid(), title: "Team 1.1.1.2"));
        teams[0].Children.ElementAt(0).Children.ElementAt(0).Children.ElementAt(1).CreationTime = DateTime.Now.AddDays(-2);

        teams[0].Children.ElementAt(0).Children.ElementAt(0).AddTeam(new(id: Guid.NewGuid(), title: "Team 1.1.1.3"));
        teams[0].Children.ElementAt(0).Children.ElementAt(0).Children.ElementAt(2).CreationTime = DateTime.Now.AddDays(-1);

        teams[0].Children.ElementAt(0).Children.ElementAt(0).AddTeam(new(id: Guid.NewGuid(), title: "Team 1.1.1.4"));
        teams[0].Children.ElementAt(0).Children.ElementAt(0).Children.ElementAt(3).CreationTime = DateTime.Now.AddDays(-1);

        await _teamRepository.InsertManyAsync(teams);

        var service = GetRequiredService<ITeamAppService>();

        // Act

        var result = await service.GetDetailAsync(teams[0].Id, depth: 4, sortChildrenBy: "CreationTime desc, Title asc");

        // Assert

        var firstChild = result.Children.ElementAt(0);
        var secondChild = result.Children.ElementAt(1);
        var first2ndChild = firstChild.Children.ElementAt(0);
        var first3rdChild = first2ndChild.Children.ElementAt(0);
        var second3rdChild = first2ndChild.Children.ElementAt(1);
        var third3rdChild = first2ndChild.Children.ElementAt(2);
        var fourth3rdChild = first2ndChild.Children.ElementAt(3);

        Assert.NotNull(result);
        Assert.NotNull(firstChild);
        Assert.NotNull(secondChild);
        Assert.NotNull(first2ndChild);
        Assert.NotNull(first3rdChild);
        Assert.NotNull(second3rdChild);

        Assert.Equal(2, result.Children.Count);
        Assert.Equal("Team 1.1", firstChild.Title);
        Assert.Equal("Team 1.2", secondChild.Title);

        Assert.Single(firstChild.Children);
        Assert.Equal("Team 1.1.1", first2ndChild.Title);

        Assert.Equal(4, first2ndChild.Children.Count);
        // now
        Assert.Equal("Team 1.1.1.1", first3rdChild.Title);
        // 1 day ago, name bigger
        Assert.Equal("Team 1.1.1.4", second3rdChild.Title);
        // 1 day ago, name smaller
        Assert.Equal("Team 1.1.1.3", third3rdChild.Title);
        // 2 days ago
        Assert.Equal("Team 1.1.1.2", fourth3rdChild.Title);
    }
}