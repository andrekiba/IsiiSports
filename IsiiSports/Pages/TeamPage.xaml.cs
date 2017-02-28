using Xamarin.Forms;

namespace IsiiSports.Pages
{
    public partial class TeamPage : ContentPage
    {
        public TeamPage()
        {
            InitializeComponent();
            PlayersList.ItemTapped += (sender, e) =>
            {
                if (Device.OS == TargetPlatform.iOS || Device.OS == TargetPlatform.Android)
                    PlayersList.SelectedItem = null;
            };

            PlayersList.ItemSelected += (sender, e) =>
            {
                PlayersList.SelectedItem = null;
            };
        }
    }
}
