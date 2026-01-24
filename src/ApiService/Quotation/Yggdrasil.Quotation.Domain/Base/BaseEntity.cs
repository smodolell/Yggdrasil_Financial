namespace Yggdrasil.Quotation.Domain.Base;

public abstract class BaseEntity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
{
    public required TKey Id { get; set; }
}
