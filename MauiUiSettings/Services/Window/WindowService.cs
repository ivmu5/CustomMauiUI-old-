using Microsoft.Maui.Platform;
using SQLiteStorage;

namespace MauiUiSettings;

public class WindowService : UiServiceBase<WindowService>, IDisposable
{
    public const double SquareTolerance = 50;

    private Window? _window;

    #region Properties

    public double Width
    {
        get => (double)GetValue(WidthProperty);
        private set => SetValue(WidthProperty, value);
    }

    public static readonly BindableProperty WidthProperty =
        CreateBindableProperty<double>(nameof(Width));


    public double Height
    {
        get => (double)GetValue(HeightProperty);
        private set => SetValue(HeightProperty, value);
    }

    public static readonly BindableProperty HeightProperty =
        CreateBindableProperty<double>(nameof(Height));


    public WindowOrientation Orientation
    {
        get => (WindowOrientation)GetValue(OrientationProperty);
        private set => SetValue(OrientationProperty, value);
    }

    public static readonly BindableProperty OrientationProperty =
        CreateBindableProperty<WindowOrientation>(nameof(Orientation));


    public bool IsVertical
    {
        get => (bool)GetValue(IsVerticalProperty);
        private set => SetValue(IsVerticalProperty, value);
    }

    public static readonly BindableProperty IsVerticalProperty =
        CreateBindableProperty<bool>(nameof(IsVertical));


    public bool IsHorizontal
    {
        get => (bool)GetValue(IsHorizontalProperty);
        private set => SetValue(IsHorizontalProperty, value);
    }

    public static readonly BindableProperty IsHorizontalProperty =
        CreateBindableProperty<bool>(nameof(IsHorizontal));


    public bool IsSquare
    {
        get => (bool)GetValue(IsSquareProperty);
        private set => SetValue(IsSquareProperty, value);
    }

    public static readonly BindableProperty IsSquareProperty =
        CreateBindableProperty<bool>(nameof(IsSquare));


    public bool IsMaximized
    {
        get => (bool)GetValue(IsMaximizedProperty);
        private set => SetValue(IsMaximizedProperty, value);
    }

    public static readonly BindableProperty IsMaximizedProperty =
        CreateBindableProperty<bool>(nameof(IsMaximized));
    #endregion



    #region Constructor

    public WindowService(IInstanceStore<UISettings> settings)
        : base(settings)
    {
    }

    #endregion

    #region Window lifecycle

    public void Attach(Window window)
    {
        Detach();

        _window = window;

        _window.SizeChanged += OnWindowSizeChanged;

        ApplySize(
            _window.Width,
            _window.Height);

#if WINDOWS
        SubscribeAppWindow();
#endif
    }

    private void Detach()
    {
        if (_window is null)
            return;

        _window.SizeChanged -= OnWindowSizeChanged;

#if WINDOWS
        UnsubscribeAppWindow();
#endif

        _window = null;
    }

    #endregion

    #region Size / Orientation

    private void OnWindowSizeChanged(
        object? sender,
        EventArgs e)
    {
        if (sender is not Window window)
            return;
#if WINDOWS
        IsMaximized = GetPresenter()!.State ==
            Microsoft.UI.Windowing.OverlappedPresenterState.Maximized;
#endif
        ApplySize(
            window.Width,
            window.Height);
    }

    private void ApplySize(
        double width,
        double height)
    {
        if (width == 0 && height == 0)
            return;

        Width = width;
        Height = height;

        UpdateOrientation(
            GetOrientation(width, height));
    }

    private void UpdateOrientation(
        WindowOrientation orientation)
    {
        Orientation = orientation;

        IsVertical =
            orientation == WindowOrientation.Vertical;

        IsHorizontal =
            orientation == WindowOrientation.Horizontal;

        IsSquare =
            orientation == WindowOrientation.Square;
    }

    private static WindowOrientation GetOrientation(
        double width,
        double height)
    {
        if (Math.Abs(width - height) < SquareTolerance)
            return WindowOrientation.Square;

        return width > height
            ? WindowOrientation.Horizontal
            : WindowOrientation.Vertical;
    }

    #endregion

    #region Window actions

    public void Minimize()
    {
#if WINDOWS
        GetPresenter()?.Minimize();
#endif
    }

    public void ToggleMaximize()
    {
#if WINDOWS
        if (GetPresenter() is not { } presenter)
            return;

        if (presenter.State ==
            Microsoft.UI.Windowing.OverlappedPresenterState.Maximized)
        {
            presenter.Restore();
        }
        else
        {
            presenter.Maximize();
        }
#endif
    }

    public void Close()
    {
        if (_window is null)
            return;

        Application.Current?.CloseWindow(_window);
    }

    #endregion

    #region Custom TitleBar

    public void SetCustomTitleBar(View? view = null)
    {
#if WINDOWS
        if (_window?.Handler?.MauiContext is not { } mauiContext)
            return;

        if (_window.Handler.PlatformView
            is not Microsoft.UI.Xaml.Window nativeWindow)
        {
            return;
        }

        if (view is null)
        {
            nativeWindow.SetTitleBar(null);
            nativeWindow.ExtendsContentIntoTitleBar = false;
            GetPresenter()?.SetBorderAndTitleBar(true, true);
            return;
        }

        if (view.Handler is null)
        {
            view.Handler = view.ToHandler(mauiContext);
        }

        if (view.Handler?.PlatformView
            is not Microsoft.UI.Xaml.UIElement nativeView)
        {
            return;
        }

        nativeWindow.ExtendsContentIntoTitleBar = true;
        nativeWindow.SetTitleBar(nativeView);
        GetPresenter()?.SetBorderAndTitleBar(false, false);
#endif
    }

    #endregion

    #region Windows

#if WINDOWS


    private Microsoft.UI.Windowing.AppWindow? AppWindow
    {
        get
        {
            if (_window?.Handler?.PlatformView
                is not MauiWinUIWindow winUIWindow)
            {
                return null;
            }

            return winUIWindow.AppWindow;
        }
    }

    private Microsoft.UI.Windowing.OverlappedPresenter? GetPresenter()
    {
        return AppWindow?.Presenter
            as Microsoft.UI.Windowing.OverlappedPresenter;
    }

    private void SubscribeAppWindow()
    {
        if (AppWindow is not { } appWindow)
            return;

        appWindow.Changed += OnAppWindowChanged;

        UpdateWindowState(appWindow);
    }

    private void UnsubscribeAppWindow()
    {
        if (AppWindow is not { } appWindow)
            return;

        appWindow.Changed -= OnAppWindowChanged;
    }

    private void OnAppWindowChanged(
        Microsoft.UI.Windowing.AppWindow sender,
        Microsoft.UI.Windowing.AppWindowChangedEventArgs args)
    {
        if (args.DidPresenterChange)
            UpdateWindowState(sender);
    }

    private void UpdateWindowState(
        Microsoft.UI.Windowing.AppWindow appWindow)
    {
        if (appWindow.Presenter
            is not Microsoft.UI.Windowing.OverlappedPresenter presenter)
        {
            IsMaximized = false;
            return;
        }

        IsMaximized =
            presenter.State ==
            Microsoft.UI.Windowing.OverlappedPresenterState.Maximized;
    }

#endif

    #endregion

    public void Dispose()
    {
        Detach();
    }
}