using Yggdrasil.Origination.Domain.Interfaces;

namespace Yggdrasil.Origination.Domain.Base;

public abstract class BaseEntityAudit<TKey> : BaseEntity<TKey>, IAuditable where TKey : IEquatable<TKey>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
