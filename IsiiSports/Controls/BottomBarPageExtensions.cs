using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IsiiSports.Controls
{
    public static class BottomBarPageExtensions
    {
        #region TabColorProperty

        public static readonly BindableProperty TabColorProperty = BindableProperty.CreateAttached(
                propertyName: "TabColor",
                returnType: typeof(Color?),
                declaringType: typeof(Page),
                defaultValue: null,
                defaultBindingMode: BindingMode.OneWay,
                validateValue: null,
                propertyChanged: null);

        public static void SetTabColor(this Page page, Color? color)
        {
            page.SetValue(TabColorProperty, color);
        }

        public static Color? GetTabColor(this Page page)
        {
            return (Color?)page.GetValue(TabColorProperty);
        }

        #endregion
    }
}
