namespace Yggdrasil.Catalog.Domain.Base;

public abstract class BaseEntity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default(TKey);
}
