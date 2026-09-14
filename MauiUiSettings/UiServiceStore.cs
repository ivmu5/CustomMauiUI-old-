using SQLiteStorage;

namespace MauiUiSettings;

public class UiServiceStore
{
    public readonly IInstanceStore<UISettings> UISettings;
    public readonly ThemeService ThemeService;
    public readonly ColorService ColorService;
    public readonly TextFontService FontService;
    public readonly LanguageService LanguageService;
    public readonly StatusBarService StatusBarService;
    public readonly CornerRadiusService CornerRadiusService;
    public readonly SnackbarService SnackbarService;
    public readonly IconFontService IconService;
    public readonly WindowService WindowService;

    public UiServiceStore(
        IInstanceStore<UISettings> uiSettings,
        ThemeService themeService,
        ColorService colorService,
        TextFontService fontService,
        LanguageService languageService,
        StatusBarService statusBarService,
        CornerRadiusService cornerRadiusService,
        SnackbarService snackbarService,
        IconFontService iconService,
        WindowService windowService)
    {
        UISettings = uiSettings;
        ThemeService = themeService;
        ColorService = colorService;
        FontService = fontService;
        LanguageService = languageService;
        StatusBarService = statusBarService;
        CornerRadiusService = cornerRadiusService;
        SnackbarService = snackbarService;
        IconService = iconService;
        WindowService = windowService;
    }
}
