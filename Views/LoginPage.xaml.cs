using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using XboxIptvSmarters.Models;
using XboxIptvSmarters.Services;

namespace XboxIptvSmarters.Views;

public sealed partial class LoginPage : Page
{
    private readonly XtreamApiService _apiService;
    
    public LoginPage()
    {
        this.InitializeComponent();
        Loaded += LoginPage_Loaded;
    }

    private async void LoginPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadCredentials();
    }

    private async Task LoadCredentials()
    {
        // Prima prova a caricare credenziali salvate
        var saved = App.Settings.GetSavedCredentials();
        if (saved.HasValue)
        {
            ServerUrlTextBox.Text = saved.Value.serverUrl;
            UsernameTextBox.Text = saved.Value.username;
            PasswordTextBox.Password = saved.Value.password;
            RememberCheckBox.IsChecked = true;
            return;
        }

        // Se non ci sono dati salvati, carica le credenziali predefinite dal file
        await LoadDefaultCredentials();
    }

    private async Task LoadDefaultCredentials()
    {
        try
        {
            // Legge appsettings.json dalla directory dell'app
            var folder = ApplicationData.Current.LocalFolder;
            
            // Controlla se esiste già il file copia locale
            StorageFile configFile = null;
            try
            {
                configFile = await folder.GetFileAsync("appsettings.json");
            }
            catch
            {
                // Se non esiste, prova a creare una copia dal packaged
                try
                {
                    var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    var sourceFile = await packageFolder.GetFileAsync("appsettings.json");
                    configFile = await sourceFile.CopyAsync(folder, "appsettings.json", NameCollisionOption.FailIfExists);
                }
                catch
                {
                    // Se fallisce tutto, prova le credenziali hardcode
                    DefaultCredentials defaultCreds = new DefaultCredentials
                    {
                        Enabled = true,
                        ServerUrl = "http://ngueyepdn.xxczsmart.com",
                        Username = "M99SVERR",
                        Password = "JJAJXT7V"
                    };
                    
                    if (defaultCreds.Enabled)
                    {
                        ServerUrlTextBox.Text = defaultCreds.ServerUrl;
                        UsernameTextBox.Text = defaultCreds.Username;
                        PasswordTextBox.Password = defaultCreds.Password;
                        RememberCheckBox.IsChecked = true;
                    }
                    return;
                }
            }
            
            if (configFile != null)
            {
                var content = await FileIO.ReadTextAsync(configFile);
                var settings = JsonSerializer.Deserialize<AppSettings>(content);
                
                if (settings != null && settings.DefaultCredentials != null && settings.DefaultCredentials.Enabled)
                {
                    ServerUrlTextBox.Text = settings.DefaultCredentials.ServerUrl;
                    UsernameTextBox.Text = settings.DefaultCredentials.Username;
                    PasswordTextBox.Password = settings.DefaultCredentials.Password;
                    RememberCheckBox.IsChecked = settings.Settings.RememberLogin;
                }
            }
        }
        catch
        {
            // Error ignorabile, user inserirà manualmente
        }
    }

    private class AppSettings
    {
        public DefaultCredentials DefaultCredentials { get; set; }
        public AppSettingsOptions Settings { get; set; }
    }

    private class DefaultCredentials
    {
        public bool Enabled { get; set; }
        public string ServerUrl { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    private class AppSettingsOptions
    {
        public bool RememberLogin { get; set; }
        public string Theme { get; set; } = "dark";
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (!XtreamApiService.ValidateCredentials(
            ServerUrlTextBox.Text, 
            UsernameTextBox.Text, 
            PasswordTextBox.Password, 
            out var errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        LoginButton.IsEnabled = false;
        LoginStatus.Text = "Connessione in corso...";

        try
        {
            var auth = new XtreamAuth
            {
                ServerUrl = ServerUrlTextBox.Text.TrimEnd('/'),
                Username = UsernameTextBox.Text,
                Password = PasswordTextBox.Password
            };
            App.CurrentAuth = auth;

            var apiService = new XtreamApiService(auth);
            var userInfo = await apiService.GetUserAsync();

            if (userInfo != null && userInfo.UserInfo.Auth == 1)
            {
                if (RememberCheckBox.IsChecked == true)
                {
                    App.Settings.SaveCredentials(
                        ServerUrlTextBox.Text,
                        UsernameTextBox.Text,
                        PasswordTextBox.Password,
                        true);
                }

                App.CurrentAuth.Status = userInfo.UserInfo.Status;
                App.CurrentAuth.ExpirationDate = userInfo.UserInfo.ExpirationDate;

                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                ShowError($"Autenticazione fallita. Status: {userInfo.UserInfo.Status} - Message: {userInfo.UserInfo.Message}");
                LoginButton.IsEnabled = true;
            }
        }
        catch (Exception ex)
        {
            ShowError($"Errore di connessione: {ex.Message}");
            LoginButton.IsEnabled = true;
        }
    }

    private void ShowError(string message)
    {
        LoginStatus.Text = message;
        LoginStatus.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
    }
}
