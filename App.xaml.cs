using Microsoft.UI.Xaml;

namespace XboxIptvSmarters;

public partial class App : Application
{
    public static XtreamAuth? CurrentAuth { get; set; }
    public static SettingsService Settings { get; } = new SettingsService();

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();
    }

    private Window m_window;
}
