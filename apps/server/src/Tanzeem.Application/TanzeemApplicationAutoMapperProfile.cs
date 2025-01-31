using AutoMapper;
using Tanzeem.Assignments;
using Tanzeem.Assignments.Dtos;
using Tanzeem.Teams;
using Tanzeem.Teams.Dtos;
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

        #region Assignment
        CreateMap<Assignment, AssignmentDto>();
        CreateMap<CreateAssignmentDto, Assignment>()
        .Ignore(x => x.Id)
        .Ignore(x => x.TenantId)
        ;
        CreateMap<UpdateAssignmentDto, Assignment>()
        .Ignore(x => x.Id)
        .Ignore(x => x.TenantId)
        ;
        CreateMap<GetAssignmentListFilter, AssignmentListFilter>();
        #endregion

        #region Team
        CreateMap<Team, TeamDto>();
        CreateMap<CreateTeamDto, Team>()
        .Ignore(x => x.Id)
        .Ignore(x => x.TenantId)
        ;
        CreateMap<UpdateTeamDto, Team>()
        .Ignore(x => x.Id)
        .Ignore(x => x.TenantId)
        ;
        CreateMap<GetTeamListFilter, TeamListFilter>();
        #endregion
    }
}
