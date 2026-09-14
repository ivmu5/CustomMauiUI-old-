using MauiUiComponents;
using SQLiteStorage;

namespace MauiUiSettings;

public static class MauiUiSettingsCollectionExtension
{
    public static IServiceCollection AddMauiUiSettings(
        this IServiceCollection services,
        SQLiteStorageOptions sqlOptions)
    {
        services.AddSingleton<UiServiceStore>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<ColorService>();
        services.AddSingleton<StatusBarService>();
        services.AddSingleton<TextFontService>();
        services.AddSingleton<LanguageService>();
        services.AddSingleton<CornerRadiusService>();
        services.AddSingleton<SnackbarService>();
        services.AddSingleton<IconFontService>();
        services.AddSingleton<WindowService>();

        services.AddSingleton(typeof(LocalizationResourceManager<>));

        sqlOptions.EntityTypes.Add(typeof(UISettings));
        sqlOptions.EntityTypes.Add(typeof(ColorDictionary));

        return services;
    }

    public static MauiAppBuilder AddMauiUiSettingsFonts(this MauiAppBuilder builder)
    {
        return builder
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Gardens CM.ttf", "Gardens CM");
                fonts.AddFont("MaterialSymbolsOutlined.ttf", "MaterialSymbolsOutlined");
                fonts.AddFont("MaterialSymbolsRounded.ttf", "MaterialSymbolsRounded");
                fonts.AddFont("MaterialSymbolsSharp.ttf", "MaterialSymbolsSharp");
            });
    }

    public async static Task InitUiSettings(this IServiceProvider services)
    {
        var repo = services.GetRequiredService<IInstanceStore<UISettings>>();
        await repo.InitAsync();
        await repo.Get().EnsureCreatedAsync(x => x.Colors);
    }
}
