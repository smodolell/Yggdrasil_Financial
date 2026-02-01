namespace Yggdrasil.Catalog.Domain.Entities;

public class Product : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;

    public bool Active { get; set; }

    public Product()
    {
        
    }
    private Product(Guid id, string name, bool active)
    {
        Id = id;
        Name = name;
        Active = active;
    }

    public static Product Create(Guid id, string name)
    {
        var result = new Product(id, name, false);


        return result;
    }


}
