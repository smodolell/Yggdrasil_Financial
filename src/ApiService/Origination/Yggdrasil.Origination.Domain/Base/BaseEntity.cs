using Yggdrasil.Origination.Domain.Interfaces;

namespace Yggdrasil.Origination.Domain.Base;

public abstract class BaseEntity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
{
    public required TKey Id { get; set; }
}
