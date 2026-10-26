using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Media.Playback;
using XboxIptvSmarters.Services;

namespace XboxIptvSmarters.Views;

public class PlayerNavigationArgs
{
    public string StreamUrl { get; set; } = "";
    public string MediaTitle { get; set; } = "";
    public string? ChannelId { get; set; }
    public string? Thumbnail { get; set; }
    public bool IsLive { get; set; }
    public int SeriesId { get; set; }
}

public sealed partial class PlayerPage : Page
{
    private readonly VideoPlayerService _playerService;
    private MediaPlayerElement? _mediaPlayer;
    private bool _controlsVisible = true;

    public PlayerPage()
    {
        this.InitializeComponent();
        _playerService = new VideoPlayerService();
        
        Loaded += PlayerPage_Loaded;
        Unloaded += PlayerPage_Unloaded;
    }

    private void PlayerPage_Loaded(object sender, RoutedEventArgs e)
    {
        var arguments = NavigationParameter as PlayerNavigationArgs;
        if (arguments != null)
        {
            SetupPlayer(arguments);
        }
    }

    private void PlayerPage_Unloaded(object sender, RoutedEventArgs e)
    {
        StopPlayback();
        _playerService.Dispose();
    }

    private void SetupPlayer(PlayerNavigationArgs args)
    {
        MediaTitle.Text = args.MediaTitle;
        
        if (args.IsLive)
        {
            LiveIndicator.Visibility = Visibility.Visible;
        }
        else
        {
            LiveIndicator.Visibility = Visibility.Collapsed;
        }

        if (!string.IsNullOrEmpty(args.Thumbnail))
        {
            try
            {
                ThumbnailImage.Source = new BitmapImage(new Uri(args.Thumbnail));
                ThumbnailImage.Visibility = Visibility.Visible;
            }
            catch
            {
                ThumbnailImage.Visibility = Visibility.Collapsed;
            }
        }
        else
        {
            ThumbnailImage.Visibility = Visibility.Collapsed;
        }

        // Setup MediaPlayerElement
        if (_mediaPlayer == null)
        {
            _mediaPlayer = new MediaPlayerElement
            {
                AutoPlay = true,
                AreTransportControlsEnabled = true,
                IsLoopingEnabled = false,
            };
            
            _mediaPlayer.TransportControls.Background = new SolidColorBrush(Windows.UI.Colors.Black);
            _mediaPlayer.TransportControls.IsCacheEnabled = true;
            _mediaPlayer.TransportControls.IsFullRowControlsEnabled = true;
            _mediaPlayer.TransportControls.IsLoopBackButtonVisible = false;
            _mediaPlayer.TransportControls.IsRepeatOneButtonVisible = false;
            _mediaPlayer.TransportControls.IsSceneMenuButtonVisible = false;
            
            MediaPlayerGrid.Children.Clear();
            MediaPlayerGrid.Children.Add(_mediaPlayer);
            
            // Event handler per nascondere controlli dopo timeout
            _mediaPlayer.Tapped += MediaPlayer_Tapped;
            _mediaPlayer.DoubleTapped += MediaPlayer_DoubleTapped;
        }

        // Load stream
        PlayStream(args.StreamUrl);
    }

    private async void PlayStream(string url)
    {
        try
        {
            StatusText.Text = "Caricamento stream...";
            LoadingProgress.Visibility = Visibility.Visible;

            if (_mediaPlayer != null)
            {
                var source = Windows.Media.Core.MediaSource.CreateFromUri(new Uri(url));
                _mediaPlayer.Source = source;
                _mediaPlayer.MediaPlayer.AudioStreamEnabled = true;
                _mediaPlayer.MediaPlayer.Play();
                
                StatusText.Text = "Stesando...";
                
                // Timeout per carico stream
                await Task.Delay(8000);
                
                if (_mediaPlayer.MediaPlayer.CurrentState == Windows.Media.Playback.MediaPlayerState.Playing)
                {
                    StatusText.Text = "Riproduzione attiva";
                    LoadingProgress.Visibility = Visibility.Collapsed;
                }
                else
                {
                    StatusText.Text = $"Errore: stato player = {_mediaPlayer.MediaPlayer.CurrentState}";
                }
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Errore riproducione: {ex.Message}";
            LoadingProgress.Visibility = Visibility.Collapsed;
        }
    }

    private void MediaPlayer_Tapped(object sender, TappedRoutedEventArgs e)
    {
        ToggleControls();
    }

    private void MediaPlayer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        ToggleFullScreen();
    }

    private void ToggleControls()
    {
        if (_controlsVisible)
        {
            ControlsPanel.Visibility = Visibility.Collapsed;
            _controlsVisible = false;
        }
        else
        {
            ControlsPanel.Visibility = Visibility.Visible;
            _controlsVisible = true;
        }
    }

    private void ToggleFullScreen()
    {
        var view = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
        view.TryEnterFullScreenMode();
        _isFullScreen = !_isFullScreen;
    }

    private bool _isFullScreen = false;

    private void StopPlayback()
    {
        if (_mediaPlayer != null)
        {
            _mediaPlayer.Source = null;
            _mediaPlayer.MediaPlayer.Pause();
        }
        
        _playerService.Stop();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        StopPlayback();
        Frame.GoBack();
    }

    private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
    {
        if (_mediaPlayer?.MediaPlayer == null) return;
        
        if (_mediaPlayer.MediaPlayer.CurrentState == Windows.Media.Playback.MediaPlayerState.Playing)
        {
            _mediaPlayer.MediaPlayer.Pause();
            PlayPauseIcon.Text = "▶";
        }
        else
        {
            _mediaPlayer.MediaPlayer.Play();
            PlayPauseIcon.Text = "⏸";
        }
    }

    private void FullScreenButton_Click(object sender, RoutedEventArgs e)
    {
        ToggleFullScreen();
        FullScreenIcon.Text = _isFullScreen ? "⛶ ESC" : "⛶ INGRESSO";
    }

    protected override void OnPointerPressed(Microsoft.UI.Input.Input.PointerRoutedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
        {
            ToggleControls();
        }
    }

    protected override void OnKeyUp(Microsoft.UI.Input.KeyRoutedEventArgs e)
    {
        base.OnKeyUp(e);
        
        switch (e.Key)
        {
            case Windows.System.VirtualKey.Escape:
                if (_isFullScreen)
                {
                    ToggleFullScreen();
                }
                else
                {
                    BackButton_Click(BackButton, new RoutedEventArgs());
                }
                break;
            case Windows.System.VirtualKey.Space:
                PlayPauseButton_Click(PlayPauseButton, new RoutedEventArgs());
                break;
            case Windows.System.VirtualKey.Left:
                ToggleControls();
                break;
            case Windows.System.VirtualKey.Right:
                ToggleControls();
                break;
        }
    }
}
