using SQLiteStorage;
using System.ComponentModel;

namespace MauiUiSettings;

public class ColorService : UiServiceBase<ColorService>, IDisposable
{
    private readonly ThemeService _themeService;

    private ColorDictionary Colors
    {
        get => _settings.Get().Colors;
        set => _settings.Get().Colors = value;
    }

    public Color Primary
    {
        get => (Color)GetValue(PrimaryProperty);
        private set => SetValue(PrimaryProperty, value);
    }
    public static readonly BindableProperty PrimaryProperty = CreateBindableProperty<Color>(nameof(Primary));

    public Color Secondary
    {
        get => (Color)GetValue(SecondaryProperty);
        private set => SetValue(SecondaryProperty, value);
    }
    public static readonly BindableProperty SecondaryProperty = CreateBindableProperty<Color>(nameof(Secondary));

    public Color Tertiary
    {
        get => (Color)GetValue(TertiaryProperty);
        private set => SetValue(TertiaryProperty, value);
    }
    public static readonly BindableProperty TertiaryProperty = CreateBindableProperty<Color>(nameof(Tertiary));

    public Color Text
    {
        get => (Color)GetValue(TextProperty);
        private set => SetValue(TextProperty, value);
    }
    public static readonly BindableProperty TextProperty = CreateBindableProperty<Color>(nameof(Text));

    public Color Background
    {
        get => (Color)GetValue(BackgroundProperty);
        private set => SetValue(BackgroundProperty, value);
    }
    public static readonly BindableProperty BackgroundProperty = CreateBindableProperty<Color>(nameof(Background));

    public Color Blur
    {
        get => (Color)GetValue(BlurProperty);
        private set => SetValue(BlurProperty, value);
    }
    public static readonly BindableProperty BlurProperty = CreateBindableProperty<Color>(nameof(Blur));



    public ColorService(
        IInstanceStore<UISettings> settings,
        ThemeService themeService)
        : base(settings)
    {
        _themeService = themeService;
        _themeService.PropertyChanged += OnThemeChanged;
        UpdateColors(_themeService.IsDark);
    }

    public void UpdateColorDictionary()
    {
        UpdateColors(_themeService.IsDark);
    }

    public void SetColorDictionary(ColorDictionary colorDictionary)
    {
        if (colorDictionary == Colors)
            return;
        Colors = colorDictionary;
        UpdateColors(_themeService.IsDark);
    }

    private void UpdateColors(bool isDark)
    {
        ColorDictionary colors = Colors;
        var (newPrimary, newSecondary, newTertiary, newText, newBackground, newBlur) = isDark
        ? (Color.FromArgb(colors.PrimaryDark),
           Color.FromArgb(colors.SecondaryDark),
           Color.FromArgb(colors.TertiaryDark),
           Color.FromArgb(colors.TextDark),
           Color.FromArgb(colors.BackgroundDark),
           Color.FromArgb(colors.BlurDark))
        : (Color.FromArgb(colors.PrimaryLight),
           Color.FromArgb(colors.SecondaryLight),
           Color.FromArgb(colors.TertiaryLight),
           Color.FromArgb(colors.TextLight),
           Color.FromArgb(colors.BackgroundLight),
           Color.FromArgb(colors.BlurLight));

        if (Primary != newPrimary) Primary = newPrimary;
        if (Secondary != newSecondary) Secondary = newSecondary;
        if (Tertiary != newTertiary) Tertiary = newTertiary;
        if (Text != newText) Text = newText;
        if (Background != newBackground) Background = newBackground;
        if (Blur != newBlur) Blur = newBlur;
    }

    public void ResetColorDictionaryAsync() => SetColorDictionary(new ColorDictionary());

    private void OnThemeChanged(object? sender, PropertyChangedEventArgs e)
    {
        UpdateColors(_themeService.IsDark);
    }

    public void Dispose()
    {
        _themeService.PropertyChanged -= OnThemeChanged;
    }
}
