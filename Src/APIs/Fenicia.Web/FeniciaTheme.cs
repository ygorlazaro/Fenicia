using MudBlazor;

namespace Fenicia.Web;

public static class FeniciaTheme
{
    public static MudTheme Current { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#5e35b1",
            PrimaryDarken = "#4527a0",
            PrimaryLighten = "#9575cd",
            PrimaryContrastText = "#ffffff",
            Secondary = "#ff6f00",
            SecondaryDarken = "#e65100",
            SecondaryLighten = "#ff8f00",
            SecondaryContrastText = "#ffffff",
            Tertiary = "#00b0ff",
            Success = "#2e7d32",
            Info = "#1565c0",
            Warning = "#ef6c00",
            Error = "#c62828",
            Dark = "#1a237e",
            Background = "#f5f5f7",
            Surface = "#ffffff",
            DrawerBackground = "#ffffff",
            DrawerText = "rgba(0,0,0,0.87)",
            AppbarBackground = "#5e35b1",
            TextPrimary = "rgba(0,0,0,0.87)",
            TextSecondary = "rgba(0,0,0,0.6)",
            ActionDefault = "rgba(0,0,0,0.54)",
            ActionDisabled = "rgba(0,0,0,0.26)",
            Divider = "rgba(0,0,0,0.08)",
            LinesDefault = "rgba(0,0,0,0.12)",
            TableLines = "rgba(0,0,0,0.06)",
            TableStriped = "rgba(0,0,0,0.02)",
            TableHover = "rgba(94, 53, 177, 0.04)",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#9575cd",
            Secondary = "#ff8f00",
            Tertiary = "#00b0ff",
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            AppbarHeight = "64px",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Inter", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "sans-serif" },
            },
            H1 = new H1Typography { FontSize = "3rem", FontWeight = "700" },
            H2 = new H2Typography { FontSize = "2.25rem", FontWeight = "700" },
            H3 = new H3Typography { FontSize = "1.75rem", FontWeight = "600" },
            H4 = new H4Typography { FontSize = "1.5rem", FontWeight = "600" },
            H5 = new H5Typography { FontSize = "1.25rem", FontWeight = "600" },
            H6 = new H6Typography { FontSize = "1.125rem", FontWeight = "600" },
            Button = new ButtonTypography { FontWeight = "600", TextTransform = "none" },
        },
    };
}
