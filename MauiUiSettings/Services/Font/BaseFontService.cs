using SQLiteStorage;

namespace MauiUiSettings;

public class BaseFontService<TService> : UiServiceBase<TService>, IFontService
{
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        internal set => SetValue(FontSizeProperty, value);
    }
    public static readonly BindableProperty FontSizeProperty = CreateBindableProperty<double>(nameof(FontSize));

    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        internal set => SetValue(FontFamilyProperty, value);
    }
    public static readonly BindableProperty FontFamilyProperty = CreateBindableProperty<string>(nameof(FontFamily));

    public Microsoft.Maui.Font CurrentFont => Microsoft.Maui.Font.OfSize(FontFamily, FontSize);
    public Microsoft.Maui.Font CurrentFontBold => Microsoft.Maui.Font.OfSize(FontFamily, FontSize, FontWeight.Bold);

    public BaseFontService(IInstanceStore<UISettings> settings)
        : base(settings) { }
}
