using IsiiSports.DataObjects;

namespace IsiiSports.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        #region Properties

        public Game Game { get; set; }

        #endregion


        #region Methods

        public override void Init(object initData)
        {
            base.Init(initData);
            if (this.Game != null)
                return;
            this.Game = (Game)initData;
        }

        #endregion
    }
}
