using SQLiteStorage;

namespace MauiUiSettings;

public class ThemeService : UiServiceBase<ThemeService>, IDisposable
{
    public const ThemeType DefaultTheme = ThemeType.System;
    public const bool DefaultThemeIsDark = true;

    public ThemeType CurrentTheme { get; private set; }
    public bool IsDark
    {
        get => (bool)GetValue(IsDarkProperty);
        private set => SetValue(IsDarkProperty, value);
    }
    public static readonly BindableProperty IsDarkProperty = CreateBindableProperty<bool>(nameof(IsDark));



    public ThemeService(IInstanceStore<UISettings> settings)
        : base(settings)
    {
        ApplyTheme(CurrentTheme = settings.Get().ThemeType);
        Application.Current!.RequestedThemeChanged += OnSystemThemeChanged;
    }

    public void SetTheme(ThemeType theme)
    {
        if (theme == CurrentTheme)
            return;
        CurrentTheme = theme;
        _settings.Get().ThemeType = theme;
        ApplyTheme(theme);
    }

    private void ApplyTheme(ThemeType theme)
    {
        IsDark = theme switch
        {
            ThemeType.Dark => true,
            ThemeType.Light => false,
            ThemeType.System => SystemThemeIsDark(),
            _ => DefaultThemeIsDark
        };
    }

    public void ResetThemeAsync() 
        => SetTheme(DefaultTheme);

    private void OnSystemThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        if (CurrentTheme == ThemeType.System)
        {
            IsDark = e.RequestedTheme == AppTheme.Unspecified
                ? DefaultThemeIsDark
                : e.RequestedTheme == AppTheme.Dark;
        }
    }

    public static bool SystemThemeIsDark()
    {
        return Application.Current!.RequestedTheme == AppTheme.Dark;
    }

    public void Dispose()
    {
        Application.Current!.RequestedThemeChanged -= OnSystemThemeChanged;
    }
}