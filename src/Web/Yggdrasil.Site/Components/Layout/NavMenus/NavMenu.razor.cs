using Microsoft.AspNetCore.Components;
using Yggdrasil.Site.Services;
using Yggdrasil.Site.Services.Dtos;


namespace Yggdrasil.Site.Components.Layout.NavMenus;

public partial class NavMenu
{

    public HashSet<AccessPointDto>? NavMenuItems { get; set; }

    [Inject]
    public ILayoutService LayoutService { get; set; } = null!;

    protected override void OnInitialized()
    {
        _layoutState.NavIsOpenEvent += () => StateHasChanged();

    }

    private async Task<HashSet<AccessPointDto>> InitMenu()
    {
        var menus = await LayoutService.GetMenu();
        return menus;

    }
    private void NavTo(AccessPointDto item)
    {
        _layoutState.NavTo(item);
    }
       protected async override Task OnAfterRenderAsync(bool firstRender)   // this doesnt update the page on load either 
    {
        if (firstRender)
        {
            NavMenuItems = await InitMenu();
            StateHasChanged();
        }
    }

}
