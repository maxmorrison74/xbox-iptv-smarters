using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace XboxIptvSmarters.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        if (App.CurrentAuth != null)
        {
            ServerUrlText.Text = App.CurrentAuth.ServerUrl;
            UsernameText.Text = App.CurrentAuth.Username;
            
            if (!string.IsNullOrEmpty(App.CurrentAuth.Status))
            {
                StatusText.Text = $"Status: {App.CurrentAuth.Status}";
            }
            
            if (!string.IsNullOrEmpty(App.CurrentAuth.ExpirationDate))
            {
                try
                {
                    var expDate = new System.DateTime(1970, 1, 1)
                        .AddSeconds(double.Parse(App.CurrentAuth.ExpirationDate))
                        .ToLocalTime();
                    ExpirationText.Text = $"Scadenza: {expDate:dd/MM/yyyy HH:mm}";
                }
                catch { }
            }
        }
        else
        {
            ServerUrlText.Text = "Nessuna sessione attiva";
            UsernameText.Text = "";
            StatusText.Text = "Offline";
            ExpirationText.Text = "";
        }

        RememberCheckBox.IsChecked = App.Settings.ShouldRememberLogin;
    }

    private void RememberLoginCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        App.Settings.SaveCredentials(
            App.CurrentAuth?.ServerUrl ?? "",
            App.CurrentAuth?.Username ?? "",
            App.CurrentAuth?.Password ?? "",
            true);
    }

    private void RememberLoginCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        // Non cancelliamo le credenziali, ma salviamo comunque con remember=false
        App.Settings.SaveCredentials(
            App.CurrentAuth?.ServerUrl ?? "",
            App.CurrentAuth?.Username ?? "",
            App.CurrentAuth?.Password ?? "",
            false);
    }

    private void ReauthenticateButton_Click(object sender, RoutedEventArgs e)
    {
        // Cancella sessione corrente
        App.CurrentAuth = null;
        Frame.Navigate(typeof(LoginPage));
    }

    private void ClearDataButton_Click(object sender, RoutedEventArgs e)
    {
        // Avviso di conferma
        var dialog = new ContentDialog
        {
            Title = "Cancella tutti i dati",
            Content = "Questa azione rimuoverà:\n- Credenziali salvate\n- Preferiti\n- Categorie selezionate\n\nSei sicuro di voler continuare?",
            PrimaryButtonText = "CANCELLA TUTTO",
            CloseButtonText = "ANNULLA",
            DefaultButton = ContentDialogButton.Close
        };

        _ = dialog.ShowAsync();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        // Esci dall'app
        App.CurrentAuth = null;
        App.Settings.DeleteSavedCredentials();
        Frame.Navigate(typeof(LoginPage));
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        // Esci dall'app
        App.CurrentAuth = null;
        Frame.Navigate(typeof(LoginPage));
    }
}
