namespace AppIntgration.Maui.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();

        // ??? ?????????
        StartAnimation();
    }

    private async void StartAnimation()
    {
        try
        {
            // ??????? ???? ????????
            await Task.WhenAll(
                IconContainer.FadeTo(1, 1000),
                IconContainer.ScaleTo(1.1, 1000, Easing.BounceOut)
            );

            // ????? ????? ???????
            await IconContainer.ScaleTo(1, 200);

            // ???? ???????
            await Task.WhenAll(
                AppTitle.FadeTo(1, 600),
                AppSubtitle.FadeTo(1, 800)
            );

            // ???? ???? ???????
            await Task.WhenAll(
                LoadingSpinner.FadeTo(1, 400),
                LoadingText.FadeTo(1, 400)
            );

            // ??????? ????? ????????
            var rotationTask = RotateIcon();

            // ?????? ???? ???????
            await Task.Delay(3000);

            // ????? ?? ???????
            LoadingText.Text = "Initializing...";
            await Task.Delay(1000);

            LoadingText.Text = "Almost ready...";
            await Task.Delay(1000);

            // ??????? ??????
            await Task.WhenAll(
                IconContainer.FadeTo(0, 500),
                AppTitle.FadeTo(0, 500),
                AppSubtitle.FadeTo(0, 500),
                LoadingSpinner.FadeTo(0, 500),
                LoadingText.FadeTo(0, 500)
            );

            // ???????? ?????? ????????
            Application.Current.MainPage = new AppShell();
        }
        catch (Exception)
        {
            // ?? ???? ???? ????? ??????
            Application.Current.MainPage = new AppShell();
        }
    }

    private async Task RotateIcon()
    {
        // ????? ???? ?????? ????????
        while (true)
        {
            await IconContainer.RotateTo(360, 8000, Easing.Linear);
            IconContainer.Rotation = 0; // ????? ????? ???????
        }
    }
}