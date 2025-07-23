using AppIntgration.Maui.Views;

namespace AppIntgration.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}