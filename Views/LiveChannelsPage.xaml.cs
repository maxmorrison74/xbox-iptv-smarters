using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using XboxIptvSmarters.Models;
using XboxIptvSmarters.Services;

namespace XboxIptvSmarters.Views;

public sealed partial class LiveChannelsPage : Page
{
    private readonly XtreamApiService _apiService;
    private ObservableCollection<LiveChannel> _allChannels = new();
    private ObservableCollection<LiveChannel> _filteredChannels = new();
    private ObservableCollection<Category> _categories = new();
    private Dictionary<string, string> _channelIcons = new();

    public LiveChannelsPage()
    {
        this.InitializeComponent();
        _apiService = new XtreamApiService(App.CurrentAuth!);
        
        Loaded += LiveChannelsPage_Loaded;
    }

    private async void LiveChannelsPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadCategoriesAndChannels();
    }

    private async Task LoadCategoriesAndChannels()
    {
        StatusText.Text = "Caricamento canali...";
        LoadingProgress.IsActive = true;

        try
        {
            // Carica categorie
            var categories = await _apiService.GetLiveCategoriesAsync();
            if (categories == null) throw new Exception("Nessuna categoria trovata");

            CategoriesList.ItemsSource = categories;
            SelectedCategory = categories.FirstOrDefault();

            // Carica tutti i canali
            var allChannels = await _apiService.GetLiveStreamsAsync();
            if (allChannels != null)
            {
                _allChannels = new ObservableCollection<LiveChannel>(allChannels);
                _filteredChannels = new ObservableCollection<LiveChannel>(allChannels);
                ChannelsList.ItemsSource = _filteredChannels;
            }

            StatusText.Text = $"{_allChannels.Count} canali caricati";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Errore: {ex.Message}";
        }
        finally
        {
            LoadingProgress.IsActive = false;
        }
    }

    private Category? SelectedCategory { get; set; }

    private void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CategoriesList.SelectedItem is Category selected)
        {
            SelectedCategory = selected;
            FilterChannelsByCategory(selected);
        }
    }

    private void FilterChannelsByCategory(Category category)
    {
        if (category.CategoryId == "all")
        {
            UpdateFilteredChannels(_allChannels);
            StatusText.Text = $"Tutti i canali: {_allChannels.Count}";
        }
        else
        {
            var filtered = _allChannels.Where(c => c.CategoryId == category.CategoryId).ToList();
            UpdateFilteredChannels(filtered);
            StatusText.Text = $"Categoria: {category.CategoryName} ({filtered.Count} canali)";
        }
    }

    private void UpdateFilteredChannels(ObservableCollection<LiveChannel> channels)
    {
        _filteredChannels.Clear();
        foreach (var channel in channels)
        {
            _filteredChannels.Add(channel);
        }
    }

    private async void ChannelListItemView_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (ChannelsList.SelectedItem is LiveChannel channel)
        {
            await PlayChannel(channel);
        }
    }

    private async Task PlayChannel(LiveChannel channel)
    {
        try
        {
            StatusText.Text = $"Riproduzione: {channel.Name}";
            LoadingProgress.IsActive = true;

            var streamUrl = await _apiService.GetLiveStreamUrl(channel.StreamId);
            var playerService = new VideoPlayerService();
            var success = await playerService.PlayAsync(streamUrl, channel.Name, null, false);

            if (success)
            {
                // Visualizza player a schermo intero
                Frame.Navigate(typeof(PlayerPage), new PlayerNavigationArgs
                {
                    StreamUrl = streamUrl,
                    MediaTitle = channel.Name,
                    ChannelId = channel.EpgChannelId,
                    IsLive = true
                });
            }
            else
            {
                StatusText.Text = "Errore nella riproduzione del canale";
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Errore: {ex.Message}";
        }
        finally
        {
            LoadingProgress.IsActive = false;
        }
    }

    private async void ChannelListItemView_DoubleTapped(object(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (ChannelsList.SelectedItem is LiveChannel channel)
        {
            await PlayChannel(channel);
        }
    }

    // Helper per ottenere icona del canale
    private async Task<BitmapImage?> GetChannelIconAsync(string? iconUrl)
    {
        if (string.IsNullOrEmpty(iconUrl)) return null;
        
        try
        {
            if (_channelIcons.TryGetValue(iconUrl, out var cached))
                return new BitmapImage(new Uri(cached));

            var bitmap = new BitmapImage();
            using var httpClient = new System.Net.Http.HttpClient();
            var byteData = await httpClient.GetByteArrayAsync(iconUrl);
            
            using var stream = byteData.AsStream();
            await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
            
            _channelIcons[iconUrl] = iconUrl;
            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    private async void LoadEPG_Click(object sender, RoutedEventArgs e)
    {
        if (ChannelsList.SelectedItem is LiveChannel channel)
        {
            if (!string.IsNullOrEmpty(channel.EpgChannelId))
            {
                var epg = await _apiService.GetShortEpgAsync(new[] { channel.EpgChannelId! });
                if (epg.TryGetValue(channel.EpgChannelId, out var programmes) && programmes.Any())
                {
                    var nowPlaying = programmes.OrderBy(p => p.Start).FirstOrDefault();
                    if (nowPlaying != null)
                    {
                        StatusText.Text = $"Attualmente: {nowPlaying.Title} - {nowPlaying.Description}";
                    }
                }
            }
        }
    }
}
