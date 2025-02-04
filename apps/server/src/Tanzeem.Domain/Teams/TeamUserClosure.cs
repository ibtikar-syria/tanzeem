using System;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Tanzeem.Teams;

public class TeamUserClosure : FullAuditedEntity<Guid>
{
    public required virtual Guid TeamId { get; set; }
    public required virtual int Depth { get; set; }
    public required virtual Guid UserId { get; set; }

    public virtual Team? Team { get; set; }
    public virtual IdentityUser? User { get; set; }

    protected TeamUserClosure() { }

    [SetsRequiredMembers]
    public TeamUserClosure(Guid id, Guid teamId, Guid userId, int depth)
    {
        Id = id;
        TeamId = teamId;
        UserId = userId;
        Depth = depth;
    }
}