using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;
using Windows.Storage;
using XboxIptvSmarters.Services;

namespace XboxIptvSmarters.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        
        Loaded += MainPage_Loaded;
    }

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        ShowLiveSection();
    }

    private void LiveButton_Click(object sender, RoutedEventArgs e)
    {
        ShowLiveSection();
    }

    private void VODButton_Click(object sender, RoutedEventArgs e)
    {
        ShowVODSection();
    }

    private void SeriesButton_Click(object sender, RoutedEventArgs e)
    {
        ShowSeriesSection();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        ShowSettingsSection();
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        App.CurrentAuth = null;
        App.Settings.DeleteSavedCredentials();
        Frame.Navigate(typeof(LoginPage));
    }

    private async Task ShowLiveSection()
    {
        ContentFrame.Navigate(typeof(LiveChannelsPage));
        
        LiveButton.IsEnabled = false;
        VODButton.IsEnabled = true;
        SeriesButton.IsEnabled = true;
        SettingsButton.IsEnabled = true;
    }

    private async Task ShowVODSection()
    {
        ContentFrame.Navigate(typeof(VodPage));
        
        LiveButton.IsEnabled = true;
        VODButton.IsEnabled = false;
        SeriesButton.IsEnabled = true;
        SettingsButton.IsEnabled = true;
    }

    private async Task ShowSeriesSection()
    {
        ContentFrame.Navigate(typeof(SeriesPage));
        
        LiveButton.IsEnabled = true;
        VODButton.IsEnabled = true;
        SeriesButton.IsEnabled = false;
        SettingsButton.IsEnabled = true;
    }

    private async Task ShowSettingsSection()
    {
        ContentFrame.Navigate(typeof(SettingsPage));
        
        LiveButton.IsEnabled = true;
        VODButton.IsEnabled = true;
        SeriesButton.IsEnabled = true;
        SettingsButton.IsEnabled = false;
    }
}
