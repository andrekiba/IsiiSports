using Xamarin.Forms;

namespace IsiiSports.Pages
{
    public partial class GamesPage : ContentPage
    {
        public GamesPage()
        {
            InitializeComponent();
            GamesView.ItemTapped += (sender, e) =>
            {
                if (Device.OS == TargetPlatform.iOS || Device.OS == TargetPlatform.Android)
                    GamesView.SelectedItem = null;
            };

            GamesView.ItemSelected += (sender, e) =>
            {
                GamesView.SelectedItem = null;
            };
        }
    }
}
