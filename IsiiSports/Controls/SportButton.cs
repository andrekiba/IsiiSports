using Xamarin.Forms;

namespace IsiiSports.Controls
{
    public class SportButton : Button
    {
        public SportButton()
        {
            const int animationTime = 100;
            Clicked += async (sender, e) =>
            {
                var btn = (SportButton)sender;
                await btn.ScaleTo(1.1, animationTime);
                await btn.ScaleTo(1, animationTime);
            };
        }
    }
}
