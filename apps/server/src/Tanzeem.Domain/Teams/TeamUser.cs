using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Tanzeem.Teams;

public class TeamUser : FullAuditedEntity<Guid>
{
    public virtual Guid TeamId { get; set; }
    public virtual Guid UserId { get; set; }

    public virtual Team Team { get; set; }
    public virtual IdentityUser User { get; set; }

    protected TeamUser() { }

    public TeamUser(Guid id, Guid teamId, Guid userId)
    {
        Id = id;
        TeamId = teamId;
        UserId = userId;
    }
}