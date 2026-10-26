using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using XboxIptvSmarters.Models;
using XboxIptvSmarters.Services;

namespace XboxIptvSmarters.Views;

public sealed partial class VodPage : Page
{
    private readonly XtreamApiService _apiService;
    private ObservableCollection<VodItem> _allVodStreams = new();
    private ObservableCollection<VodItem> _filteredVodStreams = new();
    private ObservableCollection<VodCategory> _categories = new();

    public VodPage()
    {
        this.InitializeComponent();
        _apiService = new XtreamApiService(App.CurrentAuth!);
        
        Loaded += VodPage_Loaded;
    }

    private async void VodPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadVodContents();
    }

    private async Task LoadVodContents()
    {
        StatusText.Text = "Caricamento VOD...";
        LoadingProgress.IsActive = true;

        try
        {
            // Carica categorie
            var categories = await _apiService.GetVodCategoriesAsync();
            if (categories == null) throw new Exception("Nessuna categoria VOD trovata");

            CategoriesList.ItemsSource = categories;
            SelectedCategory = categories.FirstOrDefault();

            // Carica tutti i VOD
            var vodStreams = await _apiService.GetVodStreamsAsync();
            if (vodStreams != null)
            {
                _allVodStreams = new ObservableCollection<VodItem>(vodStreams);
                _filteredVodStreams = new ObservableCollection<VodItem>(vodStreams);
                VODList.ItemsSource = _filteredVodStreams;
            }

            StatusText.Text = $"{_allVodStreams.Count} titoli caricati";
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

    private VodCategory? SelectedCategory { get; set; }

    private void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CategoriesList.SelectedItem is VodCategory selected)
        {
            SelectedCategory = selected;
            FilterVodByCategory(selected);
        }
    }

    private void FilterVodByCategory(VodCategory category)
    {
        if (category.CategoryId == "all")
        {
            UpdateFilteredChannels(_allVodStreams);
            StatusText.Text = $"Tutti i VOD: {_allVodStreams.Count}";
        }
        else
        {
            var filtered = _allVodStreams.Where(c => c.CategoryId == category.CategoryId).ToList();
            UpdateFilteredChannels(filtered);
            StatusText.Text = $"Categoria: {category.CategoryName} ({filtered.Count} titoli)";
        }
    }

    private void UpdateFilteredChannels(ObservableCollection<VodItem> streams)
    {
        _filteredVodStreams.Clear();
        foreach (var stream in streams)
        {
            _filteredVodStreams.Add(stream);
        }
    }

    private async void VODListItemView_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (VODList.SelectedItem is VodItem vod)
        {
            await PlayVod(vod);
        }
    }

    private async void VODListItemView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (VODList.SelectedItem is VodItem vod)
        {
            await PlayVod(vod);
        }
    }

    private async Task PlayVod(VodItem vod)
    {
        try
        {
            StatusText.Text = $"Riproduzione: {vod.Name}";
            LoadingProgress.IsActive = true;

            var streamUrl = await _apiService.GetVodStreamUrl(vod.StreamId);
            
            Frame.Navigate(typeof(PlayerPage), new PlayerNavigationArgs
            {
                StreamUrl = streamUrl,
                MediaTitle = vod.Name,
                Thumbnail = vod.StreamIcon,
                IsLive = false
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
}
