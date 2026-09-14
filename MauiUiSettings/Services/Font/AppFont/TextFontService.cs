using SQLiteStorage;

namespace MauiUiSettings;

public class TextFontService : BaseFontService<TextFontService>, IDisposable
{
    public const double DefaultFontSize = 14;
    public const string DefaultFontFamily = DefaultFonts.GardensCM;

    public TextFontService(IInstanceStore<UISettings> settings)
        : base(settings)
    {
        var uiSettings = _settings.Get();
        FontSize = uiSettings.FontSize;
        FontFamily = uiSettings.FontFamily;
    }

    public void SetFontFamily(string fontFamily)
    {
        FontFamily = fontFamily;
        _settings.Get().FontFamily = fontFamily;
    }

    public void SetFontSize(double fontSize)
    {
        FontSize = fontSize;
        _settings.Get().FontSize = fontSize;
    }

    public void ResetFontSize() => SetFontSize(DefaultFontSize);
    public void ResetFontFamily() => SetFontFamily(DefaultFontFamily);

    public void Dispose() { }
}