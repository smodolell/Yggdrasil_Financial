using Yggdrasil.Site.Services.Dtos;

namespace Yggdrasil.Site.Services
{
    public interface ILayoutService
    {
        Task<HashSet<AccessPointDto>> GetMenu();
    }
}