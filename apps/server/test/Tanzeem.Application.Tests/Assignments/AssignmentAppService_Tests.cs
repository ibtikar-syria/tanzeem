using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tanzeem.Assignments.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Xunit;

namespace Tanzeem.Assignments;

public abstract class AssignmentAppService_Tests<TStartupModule> : TanzeemApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IIdentityUserRepository _userRepository;

    public AssignmentAppService_Tests()
    {
        _assignmentRepository = GetRequiredService<IAssignmentRepository>();
        _userRepository = GetRequiredService<IIdentityUserRepository>();
    }

    private async Task<List<Assignment>> SeedAssignments()
    {
        var assignments = new List<Assignment>
        {
            new(id: Guid.NewGuid(), title: "Assignment 1"),
            new(id: Guid.NewGuid(), title: "Assignment 2"),
            new(id: Guid.NewGuid(), title: "Assignment 3")
        };

        foreach (var assignment in assignments)
        {
            await _assignmentRepository.InsertAsync(assignment);
        }

        return assignments;
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

    private async Task<List<Assignment>> SeedAssignmentsWithUsers()
    {
        var assignments = await SeedAssignments();

        foreach (var assignment in assignments)
        {
            var users = await SeedUsers();

            var userIds = users.Select(u => u.Id).ToList();

            await _assignmentRepository.AssignUsersAsync(assignment.Id, userIds);
        }

        assignments = await _assignmentRepository.GetListAsync(includeDetails: true);

        return assignments;
    }

    [Fact]
    public async Task Should_Create_Assignments()
    {
        var id = Guid.NewGuid();
        var title = "Assignment 1";
        var assignment = new Assignment(id, title);

        await _assignmentRepository.InsertAsync(assignment);

        var foundAssignment = await _assignmentRepository.FindAsync(id);

        Assert.NotNull(foundAssignment);
        Assert.Equal(title, foundAssignment.Title);
        Assert.Equal(id, foundAssignment.Id);
    }

    [Fact]
    public async Task Should_Assign_Users_To_Assignment()
    {
        var assignments = await SeedAssignments();

        var first = assignments[0];

        var users = await SeedUsers();

        var userIds = users.Select(u => u.Id).ToList();

        var service = GetRequiredService<IAssignmentAppService>();

        await service.AssignUsersAsync(first.Id, userIds);

        var foundAssignment = await _assignmentRepository.FindAsync(first.Id, includeDetails: true);

        Assert.NotNull(foundAssignment);
        Assert.Equal(2, foundAssignment.AssignmentUsers.Count);
        Assert.Contains(foundAssignment.AssignmentUsers, au => userIds.Contains(au.UserId));
    }

    [Fact]
    public async Task Should_Get_All_Assignments()
    {
        await SeedAssignments();

        var service = GetRequiredService<IAssignmentAppService>();
        var result = await service.GetListAsync(new());

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task Should_Filter_Assignments_By_Title()
    {
        await SeedAssignments();

        var service = GetRequiredService<IAssignmentAppService>();
        var result = await service.GetListAsync(new() { TitleContains = "Assignment 1" });

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Assignment 1", result[0].Title);
    }

    [Fact]
    public async Task Should_Filter_Assignments_By_Id()
    {
        var assignments = await SeedAssignments();

        var first = assignments[0];

        var service = GetRequiredService<IAssignmentAppService>();
        var result = await service.GetListAsync(new() { AssignmentIds = [first.Id] });

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(first.Id, result[0].Id);
    }

    [Fact]
    public async Task Should_Filter_Assignments_By_UserId()
    {
        var assignments = await SeedAssignmentsWithUsers();

        var first = assignments[0];

        var second = assignments[1];
        var secondUserIds = second!.AssignmentUsers.Select(au => au.UserId).ToList();

        var third = assignments[2];
        var thirdUserIds = third!.AssignmentUsers.Select(au => au.UserId).ToList();

        var service = GetRequiredService<IAssignmentAppService>();
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
    public async Task Should_Filter_Assignments_By_Assigned_Users()
    {
        var assignments = await SeedAssignments();

        var first = assignments[0];

        var service = GetRequiredService<IAssignmentAppService>();
        var result = await service.GetListAsync(new() { AssignmentIds = [first.Id] });

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(first.Id, result[0].Id);
    }

    [Fact]
    public async Task Should_Delete_Assignment()
    {
        var assignments = await SeedAssignments();

        var first = assignments[0];

        var service = GetRequiredService<IAssignmentAppService>();

        await service.DeleteAsync(first.Id);

        var found = await _assignmentRepository.FindAsync(first.Id);

        Assert.Null(found);
    }

    [Fact]
    public async Task Should_Update_Assignment()
    {
        var assignments = await SeedAssignments();

        var first = assignments[0];

        var service = GetRequiredService<IAssignmentAppService>();

        var updatedTitle = "Assignment 1 Updated";

        var input = new UpdateAssignmentDto(title: updatedTitle);

        var updated = await service.UpdateAsync(first.Id, input);

        Assert.NotNull(updated);
        Assert.Equal(updatedTitle, updated.Title);
    }

    [Fact]
    public async Task Should_Get_Assignment()
    {
        var assignments = await SeedAssignments();

        var first = assignments[0];

        var service = GetRequiredService<IAssignmentAppService>();

        var found = await service.GetAsync(first.Id);

        Assert.NotNull(found);
        Assert.Equal(first.Title, found.Title);
    }
}