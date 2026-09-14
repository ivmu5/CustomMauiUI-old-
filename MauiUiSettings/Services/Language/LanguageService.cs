using MauiUiSettings;
using SQLiteStorage;
using System.Globalization;

public class LanguageService : UiServiceBase<LanguageService>
{
    public const SupportedLanguage DefaultLanguage = SupportedLanguage.English;

    public SupportedLanguage Language
    {
        get => (SupportedLanguage)GetValue(LanguageProperty);
        private set => SetValue(LanguageProperty, value);
    }
    public static readonly BindableProperty LanguageProperty = CreateBindableProperty<SupportedLanguage>(nameof(Language));



    public LanguageService(IInstanceStore<UISettings> settings)
        : base(settings)
    {
        SetCulture(_settings.Get().Language);
    }

    public void SetLanguage(SupportedLanguage language)
    {
        SetCulture(language);
        _settings.Get().Language = language;
    }

    private void SetCulture(SupportedLanguage language)
    {
        var culture = new CultureInfo(language.GetCode());

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        Language = language;
    }
}