using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tanzeem.Assignments.Dtos;
using Volo.Abp.Modularity;
using Xunit;

namespace Tanzeem.Assignments;

public abstract class AssignmentAppService_Tests<TStartupModule> : TanzeemApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IAssignmentRepository _assignmentRepository;

    public AssignmentAppService_Tests()
    {
        _assignmentRepository = GetRequiredService<IAssignmentRepository>();
    }

    private async Task<List<Assignment>> SeedData()
    {
        var assignments = new List<Assignment>
        {
            new Assignment(Guid.NewGuid(), "Assignment 1"),
            new Assignment(Guid.NewGuid(), "Assignment 2"),
            new Assignment(Guid.NewGuid(), "Assignment 3")
        };

        foreach (var assignment in assignments)
        {
            await _assignmentRepository.InsertAsync(assignment);
        }

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
    public async Task Should_Get_All_Assignments()
    {
        await SeedData();

        var service = GetRequiredService<IAssignmentAppService>();
        var result = await service.GetListAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task Should_Delete_Assignment()
    {
        var assignments = await SeedData();

        var first = assignments[0];

        var service = GetRequiredService<IAssignmentAppService>();

        await service.DeleteAsync(first.Id);

        var found = await _assignmentRepository.FindAsync(first.Id);

        Assert.Null(found);
    }

    [Fact]
    public async Task Should_Update_Assignment()
    {
        var assignments = await SeedData();

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
        var assignments = await SeedData();

        var first = assignments[0];

        var service = GetRequiredService<IAssignmentAppService>();

        var found = await service.GetAsync(first.Id);

        Assert.NotNull(found);
        Assert.Equal(first.Title, found.Title);
    }
}