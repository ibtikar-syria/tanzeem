using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Tanzeem.Assignments;

public class Assignment : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Title { get; set; }

    protected Assignment()
    {
    }

    public Assignment(Guid id, string title)
    {
        Id = id;
        Title = title;
    }
}