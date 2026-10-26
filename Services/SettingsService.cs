using System.Text.Json;
using Windows.Storage;

namespace XboxIptvSmarters.Services;

public class SettingsService
{
    private const string SettingsContainerName = "AppSettings";
    private const string SavedCredentialsKey = "SavedCredentials";
    private const string RememberLoginKey = "RememberLogin";
    private const string LastCategoryKey = "LastCategory";
    private const string ThemeKey = "Theme";

    public bool IsCredentialsSaved
    {
        get
        {
            var container = ApplicationData.Current.LocalContainer;
            return container.Containers.ContainsKey(SettingsContainerName) &&
                   container.Containers[SettingsContainerName].Values.ContainsKey(SavedCredentialsKey);
        }
    }

    public void SaveCredentials(string serverUrl, string username, string password, bool rememberLogin = true)
    {
        var container = ApplicationData.Current.LocalContainer;
        if (!container.Containers.ContainsKey(SettingsContainerName))
        {
            container.CreateContainer(SettingsContainerName, ApplicationDataCreateOption.None);
        }
        
        var credentials = new
        {
            serverUrl,
            username,
            password,
            timestamp = DateTime.Now.Ticks
        };
        
        var json = JsonSerializer.Serialize(credentials);
        var credentialsContainer = container.Containers[SettingsContainerName];
        credentialsContainer.Values[SavedCredentialsKey] = json;
        credentialsContainer.Values[RememberLoginKey] = rememberLogin;
    }

    public (string serverUrl, string username, string password)? GetSavedCredentials()
    {
        var container = ApplicationData.Current.LocalContainer;
        if (!container.Containers.ContainsKey(SettingsContainerName)) return null;
        
        var credentialsContainer = container.Containers[SettingsContainerName];
        if (!credentialsContainer.Values.ContainsKey(SavedCredentialsKey)) return null;
        
        try
        {
            var json = credentialsContainer.Values[SavedCredentialsKey].ToString();
            var saved = JsonSerializer.Deserialize<SavedCredentials>(json);
            
            if (saved != null)
            {
                return (saved.serverUrl, saved.username, saved.password);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore lettura credenziali: {ex.Message}");
        }
        
        return null;
    }

    public void DeleteSavedCredentials()
    {
        var container = ApplicationData.Current.LocalContainer;
        if (!container.Containers.ContainsKey(SettingsContainerName)) return;
        
        var credentialsContainer = container.Containers[SettingsContainerName];
        credentialsContainer.Values.Remove(SavedCredentialsKey);
        credentialsContainer.Values.Remove(RememberLoginKey);
    }

    public bool ShouldRememberLogin
    {
        get
        {
            var container = ApplicationData.Current.LocalContainer;
            if (!container.Containers.ContainsKey(SettingsContainerName)) return true;
            
            var credentialsContainer = container.Containers[SettingsContainerName];
            if (!credentialsContainer.Values.ContainsKey(RememberLoginKey)) return true;
            
            return (bool)credentialsContainer.Values[RememberLoginKey];
        }
    }

    public void SaveLastCategory(string? categoryId)
    {
        var container = ApplicationData.Current.LocalContainer;
        container.Values[LastCategoryKey] = categoryId ?? "";
    }

    public string? GetLastCategory()
    {
        var container = ApplicationData.Current.LocalContainer;
        if (!container.Values.ContainsKey(LastCategoryKey)) return null;
        
        var value = container.Values[LastCategoryKey]?.ToString();
        return string.IsNullOrEmpty(value) ? null : value;
    }

    public void SaveTheme(string theme)
    {
        var container = ApplicationData.Current.LocalContainer;
        container.Values[ThemeKey] = theme;
    }

    public string GetTheme()
    {
        var container = ApplicationData.Current.LocalContainer;
        if (!container.Values.ContainsKey(ThemeKey)) return "dark";
        
        return container.Values[ThemeKey]?.ToString() ?? "dark";
    }

    public async void WipeAllData()
    {
        var container = ApplicationData.Current.LocalContainer;
        container.Values.Clear();
        container.Containers.Remove(SettingsContainerName);
        await ClearAppDataAsync();
    }

    private async Task ClearAppDataAsync()
    {
        try
        {
            var folder = ApplicationData.Current.LocalCacheFolder;
            var files = await folder.GetFilesAsync();
            foreach (var file in files)
            {
                await file.DeleteAsync();
            }
        }
        catch { }
    }

    private class SavedCredentials
    {
        public string serverUrl { get; set; } = "";
        public string username { get; set; } = "";
        public string password { get; set; } = "";
        public long timestamp { get; set; }
    }
}
