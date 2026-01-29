using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using Yggdrasil.Site.Attributes;
using Yggdrasil.Site.Services.Dtos;

namespace Yggdrasil.Site.Services;

public class LayoutService : ILayoutService
{
    public async Task<HashSet<AccessPointDto>> GetMenu()
    {
        var menus = new List<AccessPointDto>();
        var pages = GetPages(typeof(LayoutService).Assembly);
        foreach (var page in pages)
        {
            var menu = menus.FirstOrDefault(r => r.MenuName != null && r.MenuName.Equals(page.Menu));
            if (menu == null)
            {
                menu = new AccessPointDto { MenuName = page.Menu, MenuIcon = page.MenuIcon };
                menus.Add(menu);
            }
            menu.Childs.Add(new AccessPointDto
            {
                MenuName = page.MenuItem,
                Route = page.Route,
            });

        }
        return await Task.FromResult(menus.ToHashSet());
    }

    private List<PageDto> GetPages(Assembly assembly)
    {
        var components = assembly.ExportedTypes.Where(t => t.IsSubclassOf(typeof(ComponentBase)));

        var routes = components
           .Select(component => GetRouteFromComponent(component))
           .Where(config => config is not null)
           .Select(config => config!) // Ensure non-null values
           .ToList();

        return routes;
    }
    private PageDto? GetRouteFromComponent(Type component)
    {
        var attributes = component.GetCustomAttributes(inherit: true);

        var routeAttribute = attributes.OfType<RouteAttribute>().FirstOrDefault();

        if (routeAttribute is null)
        {
            // Only map routable components
            return null;
        }

        var accessPointAttribute = attributes.OfType<AccessPointAttribute>().FirstOrDefault();
        if (accessPointAttribute is null)
        {
            // Only map routable components
            return null;
        }


        var resultPage = new PageDto();

        var menu = accessPointAttribute.Menu;
        var menuIcon = accessPointAttribute.MenuIcon;
        var itemMenu = accessPointAttribute.ItemMenu;
        var accessPointType = accessPointAttribute.AccessPointType;
        var isClient = accessPointAttribute.IsClient;

        var route = routeAttribute.Template;



        if (string.IsNullOrEmpty(route))
        {
            throw new Exception($"RouteAttribute in component '{component}' has empty route template");
        }

        // Doesn't support tokens yet
        if (route.Contains('{'))
        {
            //throw new Exception($"RouteAttribute for component '{component}' contains route values. Route values are invalid for prerendering");
            route = route.Substring(0, route.IndexOf('{') - 1);

        }






        resultPage.Menu = menu;
        resultPage.MenuIcon = menuIcon;
        resultPage.MenuItem = itemMenu;
        resultPage.Route = route;
        resultPage.AccessPointType = accessPointType;
        resultPage.IsAnonymous = attributes.OfType<AllowAnonymousAttribute>().FirstOrDefault() != null;

        return resultPage;
    }
}
