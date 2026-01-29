using Microsoft.AspNetCore.Components;
using Yggdrasil.Site.Services.Dtos;

namespace Yggdrasil.Site.Components.Layout.NavMenus;

public partial class NavItemMenu
{
    [Parameter] public HashSet<AccessPointDto> NavMenuItems { get; set; } = new();

    [Parameter] public EventCallback<AccessPointDto> NavTo { get; set; }

    private bool _shouldRender = false;



    protected override bool ShouldRender() => _shouldRender;

    private async Task NavClick(AccessPointDto item)
    {
        await NavTo.InvokeAsync(item);
    }

}
