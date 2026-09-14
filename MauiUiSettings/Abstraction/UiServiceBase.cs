using SQLiteStorage;

namespace MauiUiSettings;

public abstract class UiServiceBase<TService> : BindableObject
{
    protected readonly IInstanceStore<UISettings> _settings;


    public UiServiceBase(IInstanceStore<UISettings> settings)
        => _settings = settings;

    public async Task SaveAsync()
        => await _settings.SaveAsync();

    protected static BindableProperty CreateBindableProperty<T>(string name, object? defaultValue = null)
        => BindableProperty.Create(name, typeof(T), typeof(TService), defaultValue);
}
