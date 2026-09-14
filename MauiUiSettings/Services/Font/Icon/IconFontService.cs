using SQLiteStorage;
using System.ComponentModel;

namespace MauiUiSettings;

public class IconFontService : BaseFontService<IconFontService>, IDisposable
{
    public const MaterialSymbolMode DefaultIconFamilyMode = MaterialSymbolMode.Auto;
    public const string DefaultIconFamily = DefaultFonts.MaterialSymbolSharp;
    public const double DefaultIconSize = 24;

    private readonly CornerRadiusService _cornerRadiusService;

    public MaterialSymbolMode IconFamilyMode { get; private set; }



    public IconFontService(
        IInstanceStore<UISettings> settings,
        CornerRadiusService cornerRadiusService)
        : base(settings)
    {
        _cornerRadiusService = cornerRadiusService;
        _cornerRadiusService.PropertyChanged += OnCornerRadiusChanged;

        var uiSettings = _settings.Get();

        UpdateIconFamily(uiSettings.IconFamilyMode);
        FontSize = uiSettings.IconSize;
    }

    public void SetIconFamily(MaterialSymbolMode iconFamily)
    {
        UpdateIconFamily(iconFamily);
        _settings.Get().IconFamilyMode = iconFamily;
    }

    private void UpdateIconFamily(MaterialSymbolMode iconFamily)
    {
        IconFamilyMode = iconFamily;
        FontFamily = IconFamilyName(iconFamily);
    }

    public void SetIconSize(double iconSize)
    {
        FontSize = iconSize;
        _settings.Get().IconSize = iconSize;
    }



    public void ResetIconFamily()
        => SetIconFamily(DefaultIconFamilyMode);

    public void ResetIconSize()
        => SetIconSize(DefaultIconSize);



    public void OnCornerRadiusChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(CornerRadiusService.CornerRadius))
            return;

        var iconFamilyMode = _settings.Get().IconFamilyMode;
        if (iconFamilyMode == MaterialSymbolMode.Auto)
            SetIconFamily(iconFamilyMode);
    }

    public string IconFamilyName(MaterialSymbolMode iconFamilyMode)
    {
        if (iconFamilyMode == MaterialSymbolMode.Auto)
            return _cornerRadiusService.CornerRadius switch
            {
                < 5 => DefaultFonts.MaterialSymbolSharp,
                < 10 => DefaultFonts.MaterialSymbolOutlined,
                _ => DefaultFonts.MaterialSymbolRounded
            };

        return iconFamilyMode.GetFontName();
    }

    public void Dispose()
    {
        _cornerRadiusService.PropertyChanged -= OnCornerRadiusChanged;
    }
}
