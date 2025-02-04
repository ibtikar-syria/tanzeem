using System;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Entities.Auditing;

namespace Tanzeem.Teams;

public class TeamClosure : FullAuditedEntity<Guid>
{
    public required virtual Guid TeamId { get; set; }
    // depth from the parent team
    public required virtual int Depth { get; set; }
    public required virtual Guid ChildTeamId { get; set; }

    public virtual Team? Team { get; set; }
    public virtual Team? ChildTeam { get; set; }

    protected TeamClosure() { }

    [SetsRequiredMembers]
    public TeamClosure(Guid id, Guid teamId, Guid childTeamId, int depth)
    {
        Id = id;
        TeamId = teamId;
        ChildTeamId = childTeamId;
        Depth = depth;
    }
}