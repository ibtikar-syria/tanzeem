using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Tanzeem.Assignments;

public class AssignmentUser : FullAuditedEntity<Guid>
{
    public virtual Guid AssignmentId { get; set; }
    public virtual Guid UserId { get; set; }

    public virtual Assignment Assignment { get; set; }
    public virtual IdentityUser User { get; set; }

    protected AssignmentUser() { }

    public AssignmentUser(Guid id, Guid assignmentId, Guid userId)
    {
        Id = id;
        AssignmentId = assignmentId;
        UserId = userId;
    }
}