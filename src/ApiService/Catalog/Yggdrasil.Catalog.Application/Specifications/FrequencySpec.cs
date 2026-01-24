namespace Yggdrasil.Catalog.Application.Specifications;
public class FrequencySpec : Specification<Frequency>
{

    public FrequencySpec(string? searchText, bool? isActive)
    {
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            Query.Where(f =>
                f.Name.Contains(searchText) ||
                f.Code.Contains(searchText) ||
                f.Description.Contains(searchText));
        }

        if (isActive.HasValue)
        {
            Query.Where(f => f.IsActive == isActive.Value);
        }

        // Orden por defecto
        Query.OrderBy(f => f.Name);
    }
}
