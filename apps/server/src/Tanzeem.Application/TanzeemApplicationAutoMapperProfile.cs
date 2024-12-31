using AutoMapper;
using Volo.Abp.AutoMapper;

namespace Tanzeem;

public class TanzeemApplicationAutoMapperProfile : Profile
{
    public TanzeemApplicationAutoMapperProfile()
    {
        MapAssignment();
    }

    private void MapAssignment()
    {
        CreateMap<Assignments.Assignment, Assignments.Dtos.AssignmentDto>();
        CreateMap<Assignments.Dtos.CreateAssignmentDto, Assignments.Assignment>()
        .Ignore(x => x.Id)
        .Ignore(x => x.TenantId)
        ;
    }
}
