namespace Yggdrasil.Quotation.Domain.Interfaces;

public interface IEntity<TKey> where TKey : IEquatable<TKey>
{
    TKey Id { get; set; }
}
