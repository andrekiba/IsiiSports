using IsiiSports.DataObjects;

namespace IsiiSports.ViewModels
{
    public class GameViewModel : BaseViewModel
    {

        #region Methods

        public override void Init(object initData)
        {
            var game = (Game) initData;
            base.CoreMethods.DisplayAlert("Arguments", $"{game?.Team1?.Name} vs {game?.Team2?.Name}", "Ok");
            base.Init(initData);
        }

        #endregion
    }
}
