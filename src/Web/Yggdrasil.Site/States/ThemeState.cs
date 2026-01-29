using BitzArt.Blazor.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using MudBlazor;
using MudBlazor.Utilities;

namespace Yggdrasil.Site.States;

public class ThemeState
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly NavigationManager _navigationManager;

    private readonly ICookieService _cookieService;

    public ThemeState(IHttpContextAccessor httpContextAccessor,
        NavigationManager navigationManager,
        ICookieService cookieService)
    {
        _httpContextAccessor = httpContextAccessor;
        _navigationManager = navigationManager;
        _cookieService = cookieService;
        var value = _httpContextAccessor.HttpContext?.Request.Cookies.Where(c => c.Key == "IsDark")
            .FirstOrDefault().Value;
        _isDark = string.IsNullOrEmpty(value) ? false : bool.Parse(value);

        var primaryColor = _httpContextAccessor.HttpContext?.Request.Cookies.Where(c => c.Key == "PrimaryColor")
            .FirstOrDefault().Value;

        _theme = new MudTheme()
        {
            PaletteLight = new PaletteLight
            {
                Primary = Colors.Purple.Default,
                Secondary = Colors.Blue.Default,
                SecondaryDarken = Colors.Blue.Default,
                SecondaryLighten = Colors.Blue.Default,
            },
            PaletteDark = new PaletteDark()
            {
                Primary = Colors.Purple.Default,
                Secondary = Colors.Blue.Default,
                SecondaryDarken = Colors.Blue.Default,
                SecondaryLighten = Colors.Blue.Default,
            },
            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "0px",
            },
            Typography = new Typography()
            {
                Default = new DefaultTypography()
                {
                    FontFamily = new[] { "Roboto", "sans-serif" },
                    FontSize = "0.775rem",
                },
               
                H1 = new H1Typography()
                {
                    FontSize = "2rem",  // 2 veces el tamaño base
                    FontWeight = "600",
                    LineHeight = "1.2"
                },
                H2 = new H2Typography()
                {
                    FontSize = "1.75rem", // 1.75 veces el tamaño base
                    FontWeight = "500",
                    LineHeight ="1.3"
                },
                H3 = new H3Typography()
                {
                    FontSize = "1.5rem", // 1.5 veces el tamaño base
                    FontWeight ="500",
                    LineHeight = "1.3"
                },
                H4 = new H4Typography()
                {
                    FontSize = "1.25rem", // 1.25 veces el tamaño base
                    FontWeight = "500",
                    LineHeight = "1.3"
                },
                H5 = new H5Typography()
                {
                    FontSize = "1.125rem", // 1.125 veces el tamaño base
                    FontWeight = "500",
                    LineHeight = "1.3"
                },
                H6 = new H6Typography()
                {
                    FontSize = "1rem", // Igual al tamaño base
                    FontWeight = "500",
                    LineHeight = "1.3"
                }
            }
        };

        if (string.IsNullOrEmpty(primaryColor))
        {
            primaryColor = "#1668dc";
        }

        var color = new MudColor(primaryColor);
        UpdatePaletteColor(color);
    }

    private void UpdatePaletteColor(MudColor color)
    {
        _theme.PaletteLight.Primary = color;
        _theme.PaletteLight.PrimaryDarken = color.ColorRgbDarken().ToString(MudColorOutputFormats.RGB);
        _theme.PaletteLight.PrimaryLighten = color.ColorRgbLighten().ToString(MudColorOutputFormats.RGB);
        _theme.PaletteLight.AppbarBackground = color;

        _theme.PaletteDark.Primary = color;
        _theme.PaletteDark.PrimaryDarken = color.ColorRgbDarken().ToString(MudColorOutputFormats.RGB);
        _theme.PaletteDark.PrimaryLighten = color.ColorRgbLighten().ToString(MudColorOutputFormats.RGB);
    }

    private bool _isDark;

    private MudTheme _theme = new();

    public event Action? ThemeChangeEvent;

    public event Action? IsDarkChangeEvent;

    public event Action? IsPluginChangeEvent;

    public void LoadTheme()
    {
        IsDarkStateChanged();
        ThemeStateChanged();
        IsPluginStateChanged();
    }

    public bool IsDark
    {
        get
        {
            return _isDark;
        }
        set
        {
            _isDark = value;
            _cookieService.SetAsync("IsDark", value.ToString());
            LoadTheme();
        }
    }


    public MudColor PrimaryColor
    {
        get
        {
            return _theme.PaletteLight.Primary;
        }
        set
        {
            UpdatePaletteColor(value);
            _cookieService.SetAsync("PrimaryColor", value.ToString(MudColorOutputFormats.Hex));
            LoadTheme();
        }
    }


    public MudTheme MudTheme
    {
        get
        {
            return _theme;
        }
    }

    private void ThemeStateChanged() => ThemeChangeEvent?.Invoke();
    private void IsDarkStateChanged() => IsDarkChangeEvent?.Invoke();
    private void IsPluginStateChanged() => IsPluginChangeEvent?.Invoke();

}
