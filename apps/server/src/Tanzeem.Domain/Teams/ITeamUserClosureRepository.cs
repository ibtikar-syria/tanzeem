using System;
using Volo.Abp.Domain.Repositories;

namespace Tanzeem.Teams;

public interface ITeamUserClosureRepository : IRepository<TeamUserClosure, Guid>
{
}
