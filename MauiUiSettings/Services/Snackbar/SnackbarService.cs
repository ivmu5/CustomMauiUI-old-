using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using SQLiteStorage;

namespace MauiUiSettings;

public class SnackbarService
{
    public const int DefaultMaxQueueSize = 25;

    private readonly CornerRadiusService _cornerRadiusService;
    private readonly TextFontService _fontService;
    private readonly ColorService _colorService;

    private readonly LinkedList<SnackbarMessage> _messages = new();
    private readonly object _lock = new();
    private readonly int _maxQueueSize;
    private bool _isProcessing = false;

    private static IView? BaseAnchor;


    public SnackbarService(
        CornerRadiusService cornerRadiusService,
        TextFontService fontService,
        ColorService colorService,
        IInstanceStore<UISettings> settings)
    {
        _cornerRadiusService = cornerRadiusService;
        _fontService = fontService;
        _colorService = colorService;
        _maxQueueSize = settings.Get().MaxSnackbarQueueSize;
    }

    public static void SetBaseAnchor(IView? view = null)
    {
        BaseAnchor = view;
    }

    public void Info(string message)
    {
        Show(new SnackbarMessage()
        {
            Message = message,
            Type = SnackbarType.Info
        });
    }

    public void Success(string message)
    {
        Show(new SnackbarMessage()
        {
            Message = message,
            Type = SnackbarType.Success
        });
    }

    public void Warning(string message)
    {
        Show(new SnackbarMessage()
        {
            Message = message,
            Type = SnackbarType.Warning
        });
    }

    public void Error(string message)
    {
        Show(new SnackbarMessage()
        {
            Message = message,
            Type = SnackbarType.Error
        });
    }

    public void Show(SnackbarMessage message)
    {
#if !(WINDOWS10_0_17763_0_OR_GREATER || MACCATALYST15_0_OR_GREATER || ANDROID || IOS)
    return;
#endif
        lock (_lock)
        {
            if (_messages.Count >= _maxQueueSize)
                _messages.RemoveLast();

            if (message.IsPriority)
                _messages.AddFirst(message);
            else
                _messages.AddLast(message);

            if (_isProcessing)
                return;

            _isProcessing = true;
        }

        _ = ProcessQueueAsync();
    }

    private async Task ProcessQueueAsync()
    {
        while (true)
        {
            SnackbarMessage? message = null;

            lock (_lock)
            {
                if (_messages.First == null)
                    break;

                message = _messages.First.Value;
                _messages.RemoveFirst();
            }

            try
            {
                await ShowMessageAsync(message);
            }
            catch
            {
            }

            await Task.Delay(1500);
        }
        lock (_lock)
        {
            _isProcessing = false;
        }
    }

    private async Task ShowMessageAsync(SnackbarMessage message)
    {
        var options = new SnackbarOptions
        {
            BackgroundColor = GetBackgroundColor(message.Type),
            TextColor = _colorService.Text,
            ActionButtonTextColor = GetActionButtonTextColor(message.Type),

            CornerRadius = new CornerRadius(_cornerRadiusService.CornerRadius),

            Font = _fontService.CurrentFont,
            ActionButtonFont = _fontService.CurrentFontBold
        };
        var anchor = message.Anchor ?? BaseAnchor;
        var snackbar = Snackbar.Make(
            message.Message,
            async () =>
            {
                if (message.Action != null)
                    await message.Action();
            },
            message.ActionText,
            message.Duration,
            options,
            anchor);

        await snackbar.Show();
    }

    private Color GetBackgroundColor(SnackbarType priority)
    {
        return priority switch
        {
            SnackbarType.Info => _colorService.Tertiary,
            SnackbarType.Success => _colorService.Secondary,
            SnackbarType.Warning => _colorService.Primary,
            SnackbarType.Error => Color.FromArgb("#EF4444"),
            _ => _colorService.Secondary
        };
    }

    private Color GetActionButtonTextColor(SnackbarType priority)
    {
        return priority switch
        {
            SnackbarType.Warning => _colorService.Secondary,
            _ => _colorService.Primary
        };
    }
}