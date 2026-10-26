using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using XboxIptvSmarters.Models;
using XboxIptvSmarters.Services;

namespace XboxIptvSmarters.Views;

public sealed partial class SeriesPage : Page
{
    private readonly XtreamApiService _apiService;
    private ObservableCollection<SeriesData> _allSeries = new();
    private SelectedSeries? _currentSelectedSeries;
    
    public class SelectedSeries
    {
        public SeriesData SeriesData { get; set; } = new();
        public Season? SelectedSeason { get; set; }
        public Episode? SelectedEpisode { get; set; }
    }

    public SeriesPage()
    {
        this.InitializeComponent();
        _apiService = new XtreamApiService(App.CurrentAuth!);
        
        Loaded += SeriesPage_Loaded;
    }

    private async void SeriesPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadSeries();
    }

    private async Task LoadSeries()
    {
        StatusText.Text = "Caricamento serie TV...";
        LoadingProgress.IsActive = true;

        try
        {
            var seriesList = await _apiService.GetSeriesAsync();
            if (seriesList == null) throw new Exception("Nessuna serie TV trovata");

            _allSeries = new ObservableCollection<SeriesData>(seriesList);
            SeriesList.ItemsSource = _allSeries;

            StatusText.Text = $"{_allSeries.Count} serie caricate";
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

    private async void SeriesListItemView_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (SeriesList.SelectedItem is SeriesData series)
        {
            await ShowSeriesDetails(series);
        }
    }

    private async void SeriesListItemView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (SeriesList.SelectedItem is SeriesData series)
        {
            await ShowSeriesDetails(series);
        }
    }

    private async Task ShowSeriesDetails(SeriesData series)
    {
        StatusText.Text = $"Caricamento: {series.Name}";

        // Mostra dettaglio serie con stagioni
        DetailsPanel.Visibility = Visibility.Visible;
        SeriesDetailsTitle.Text = series.Name;
        
        if (!string.IsNullOrEmpty(series.Cover))
        {
            SeriesCover.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(series.Cover));
        }

        if (series.Seasons != null && series.Seasons.Any())
        {
            // Prima stagione di default
            var firstSeason = series.Seasons.Values.First();
            SeasonList.ItemsSource = series.Seasons.Values.ToList();
            SelectedSeason = firstSeason;
            EpisodeList.ItemsSource = firstSeason.Episodes;
            
            if (firstSeason.Episodes.Any())
            {
                SelectedEpisode = firstSeason.Episodes.First();
                EpisodeTitle.Text = SelectedEpisode.Name;
                EpisodeDescription.Text = SelectedEpisode.Info?.Plot ?? "Nessuna descrizione disponibile";
            }
        }
        else
        {
            StatusText.Text = "Nessuna stagione disponibile per questa serie";
        }
    }

    private void Season_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SeasonList.SelectedItem is Season season)
        {
            SelectedSeason = season;
            EpisodeList.ItemsSource = season.Episodes;
            
            if (season.Episodes.Any())
            {
                SelectedEpisode = season.Episodes.First();
                EpisodeTitle.Text = SelectedEpisode.Name;
                EpisodeDescription.Text = SelectedEpisode.Info?.Plot ?? "Nessuna descrizione disponibile";
            }
        }
    }

    private async void EpisodeItemView_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (EpisodeList.SelectedItem is Episode episode)
        {
            await PlayEpisode(episode);
        }
    }

    private async void EpisodeItemView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (EpisodeList.SelectedItem is Episode episode)
        {
            await PlayEpisode(episode);
        }
    }

    private async Task PlayEpisode(Episode episode)
    {
        try
        {
            StatusText.Text = $"Riproduzione: {SelectedSeries?.SeriesData.Name} - S{SelectedSeason?.SeasonNumber}E{episode.EpisodeNumber}";
            LoadingProgress.IsActive = true;

            // Usa stream_id della serie + numero stagione
            var streamUrl = await _apiService.GetSeriesStreamUrl(
                SelectedSeason!.SeasonNumber,
                episode.EpisodeNumber,
                _currentSelectedSeries!.SeriesData.SeriesId);
            
            Frame.Navigate(typeof(PlayerPage), new PlayerNavigationArgs
            {
                StreamUrl = streamUrl,
                MediaTitle = $"{SelectedSeries?.SeriesData.Name} - S{SelectedSeason?.SeasonNumber}E{episode.EpisodeNumber}: {episode.Name}",
                IsLive = false,
                SeriesId = _currentSelectedSeries.seriesData.SeriesId
            });
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

    private void BackToList_Click(object sender, RoutedEventArgs e)
    {
        DetailsPanel.Visibility = Visibility.Collapsed;
        SelectedSeries = null;
    }
}
