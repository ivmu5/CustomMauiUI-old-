using Microsoft.Maui.Controls.Shapes;
using SQLiteStorage;

namespace MauiUiSettings;

public class CornerRadiusService : UiServiceBase<CornerRadiusService>
{
    public const int MinCornerRadius = 0;
    public const int MaxCornerRadius = 25;
    public const int DefaultCornerRadius = 17;

    public int CornerRadius
    {
        get => (int)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
    public static readonly BindableProperty CornerRadiusProperty = CreateBindableProperty<int>(nameof(CornerRadius));

    public RoundRectangle RoundRectangle
    {
        get => (RoundRectangle)GetValue(RoundRectangleProperty);
        private set => SetValue(RoundRectangleProperty, value);
    }
    public static readonly BindableProperty RoundRectangleProperty = CreateBindableProperty<RoundRectangle>(nameof(RoundRectangle));



    public CornerRadiusService(IInstanceStore<UISettings> settings)
        : base(settings)
    {
        ApplyCornerRadius(_settings.Get().CornerRadius);
    }

    public void SetCornerRadius(int radius)
    {
        ApplyCornerRadius(radius);
        _settings.Get().CornerRadius = radius;
    }

    private void ApplyCornerRadius(int radius)
    {
        CornerRadius = radius;
        RoundRectangle = new RoundRectangle()
        {
            CornerRadius = radius
        };
    }

    public void ResetCornerRadius() => SetCornerRadius(DefaultCornerRadius);
}