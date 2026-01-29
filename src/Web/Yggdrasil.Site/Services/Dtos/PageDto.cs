using Yggdrasil.Site.Attributes;

namespace Yggdrasil.Site.Services.Dtos;

internal class PageDto
{
    public string Menu { get; set; } = "";
    public string MenuIcon { get; set; } = "";
    public string MenuItem { get; set; } = "";
    public string Route { get; set; } = "";
    public bool IsAnonymous { get; set; }
    public AccessPointType AccessPointType { get; set; }
    public List<PageElementDto> PageElements { get; set; } = new List<PageElementDto>();
}

internal class PluginDto
{
    public Guid Id { get; set; }
    public string PluginName { get; set; } = "";
    public string Description { get; set; } = "";
    public List<PageDto> Pages { get; set; } = new List<PageDto>();



}

internal class PageElementDto
{

    public string Identify { get; set; } = "";
}

