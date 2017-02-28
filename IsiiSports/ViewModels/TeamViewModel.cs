using System;
using IsiiSports.DataObjects;

namespace IsiiSports.ViewModels
{
    public class TeamViewModel : BaseViewModel
    {
        #region Properties

        public Team Team { get; set; }

        #endregion

        #region Methods

        public override void Init(object initData)
        {
            base.Init(initData);
            if (this.Team != null)
                return;
            this.Team = (Team)initData;
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {

        }

        #endregion


        #region Commands

        

        #endregion
    }
}
