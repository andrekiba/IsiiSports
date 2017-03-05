using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IsiiSports.Controls
{
    public class BottomBarPage : TabbedPage
    {
        public enum BarThemeTypes { Light, DarkWithAlpha, DarkWithoutAlpha }

        public bool FixedMode { get; set; }
        public BarThemeTypes BarTheme { get; set; }

        public void RaiseCurrentPageChanged()
        {
            OnCurrentPageChanged();
        }
    }
}
