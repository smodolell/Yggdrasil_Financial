using MudBlazor;
using Yggdrasil.Site.Constants;

namespace Yggdrasil.Site.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AccessPointAttribute : Attribute
{
    public string Menu { get; set; } = "";
    public string ItemMenu { get; set; } = "";

    public string MenuIcon => _menuIcon();

    public AccessPointType AccessPointType { get; set; }

    public bool IsClient { get; set; }

    public AccessPointAttribute(string menu, string itemMenu)
    {
        Menu = menu;
        ItemMenu = itemMenu;
        AccessPointType = AccessPointType.LeftMenu;
        IsClient = false;
    }

    public AccessPointAttribute(string menu, string itemMenu, AccessPointType accessPointType)
    {
        Menu = menu;
        ItemMenu = itemMenu;
        AccessPointType = accessPointType;
        IsClient = false;
    }

    public AccessPointAttribute(string menu, string itemMenu, AccessPointType accessPointType, bool isClient) : this(menu, itemMenu, accessPointType)
    {
        IsClient = isClient;
    }

    private string _menuIcon()
    {
        switch (Menu)
        {
            case AppMenu.MenuConfiguracion:
                return AppMenu.MenuConfiguracionIcon;
            case AppMenu.MenuSistema:
                return AppMenu.MenuSistemaIcon;
            case AppMenu.MenuSeguridad:
                return AppMenu.MenuSeguridadIcon;
            case AppMenu.MenuRegistro:
                return Icons.Material.Filled.Info;
            case AppMenu.MenuCatalogo:
                return AppMenu.MenuCatalogoIcon;
            case AppMenu.MenuOperacion:
                return AppMenu.MenuOperacionIcon;
            case AppMenu.MenuProceso:
                return AppMenu.MenuProcesoIcon;
            case AppMenu.MenuCliente:
                return Icons.Material.Filled.Person;
            case AppMenu.MenuAlerta:
                return Icons.Material.Filled.Warning;
            case AppMenu.MenuReporte:
                return AppMenu.MenuReporteIcon;
            case AppMenu.MenuDevelopment:
                return AppMenu.MenuDevelopmentIcon;
            case AppMenu.MenuHome:
                return Icons.Material.Filled.Home;
            case AppMenu.MenuOtrorgamiento:
                return AppMenu.MenuOtrorgamientoIcon;
            case AppMenu.MenuCobranza:
                return AppMenu.MenuCobranzaIcon;
            case AppMenu.MenuPortalCliente:
                return AppMenu.MenuPortalClienteIcon;
            case AppMenu.MenuTest:
                return AppMenu.MenuTestIcon;
            case AppMenu.MenuGeneral:
                return AppMenu.MenuGeneralIcon;
            case AppMenu.MenuAsignacion:
                return AppMenu.MenuAsignacionIcon;
            case AppMenu.MenuPlatform:
                return AppMenu.MenuPlatformIcon;
            case AppMenu.MenuApoyo:
                return AppMenu.MenuApoyoIcon;
             case AppMenu.MenuDashboad:
                return AppMenu.MenuDashboadIcon;
            case AppMenu.MenuGestion:
                return AppMenu.MenuGestionIcon;
            default: return "";
        }
    }
}

public enum AccessPointType
{
    LeftMenu,
    Page,
    Element

}
