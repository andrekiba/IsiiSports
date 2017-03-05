using System;
using System.ComponentModel;
using System.Linq;
using Android.Views;
using Android.Widget;
using BottomNavigationBar;
using BottomNavigationBar.Listeners;
using IsiiSports.Controls;
using IsiiSports.Droid.Renderers;
using IsiiSports.Droid.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using IPageController = IsiiSports.Droid.Utils.IPageController;

[assembly: ExportRenderer(typeof (BottomBarPage), typeof (BottomBarPageRenderer))]
namespace IsiiSports.Droid.Renderers
{
    public class BottomBarPageRenderer : VisualElementRenderer<BottomBarPage>, IOnTabClickListener
    {
        private bool disposed;
        private BottomBar bottomBar;
        private FrameLayout frameLayout;
        private IPageController pageController;

        public BottomBarPageRenderer()
        {
            AutoPackage = false;
        }

        #region IOnTabClickListener

        public void OnTabSelected(int position)
        {
            //Do we need this call? It's also done in OnElementPropertyChanged
            SwitchContent(Element.Children[position]);
            var bottomBarPage = Element;
            bottomBarPage.CurrentPage = Element.Children[position];
            //bottomBarPage.RaiseCurrentPageChanged();
        }

        public void OnTabReSelected(int position)
        {
        }
        
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;

                RemoveAllViews();

                foreach (var pageToRemove in Element.Children)
                {
                    var pageRenderer = Platform.GetRenderer(pageToRemove);

                    if (pageRenderer != null)
                    {
                        pageRenderer.ViewGroup.RemoveFromParent();
                        pageRenderer.Dispose();
                    }

                    // pageToRemove.ClearValue (Platform.RendererProperty);
                }

                if (bottomBar != null)
                {
                    bottomBar.SetOnTabClickListener(null);
                    bottomBar.Dispose();
                    bottomBar = null;
                }

                if (frameLayout != null)
                {
                    frameLayout.Dispose();
                    frameLayout = null;
                }

                /*if (Element != null) {
					PageController.InternalChildren.CollectionChanged -= OnChildrenCollectionChanged;
				}*/
            }

            base.Dispose(disposing);
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            pageController.SendAppearing();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            pageController.SendDisappearing();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BottomBarPage> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {

                var bottomBarPage = e.NewElement;

                if (bottomBar == null)
                {
                    pageController = PageController.Create(bottomBarPage);

                    // create a view which will act as container for Page's
                    frameLayout = new FrameLayout(Forms.Context)
                    {
                        LayoutParameters = new FrameLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent, GravityFlags.Fill)
                    };
                    AddView(frameLayout, 0);

                    // create bottomBar control
                    bottomBar = BottomBar.Attach(frameLayout, null);
                    bottomBar.NoTabletGoodness();
					bottomBar.MaxFixedTabCount = Element.Children.Count - 1;
                    if (bottomBarPage.FixedMode)
                    {
                        bottomBar.UseFixedMode();
                    }

                    switch (bottomBarPage.BarTheme)
                    {
                        case BottomBarPage.BarThemeTypes.Light:
                            break;
                        case BottomBarPage.BarThemeTypes.DarkWithAlpha:
                            bottomBar.UseDarkThemeWithAlpha();
                            break;
                        case BottomBarPage.BarThemeTypes.DarkWithoutAlpha:
                            bottomBar.UseDarkThemeWithAlpha(false);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    bottomBar.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    bottomBar.SetOnTabClickListener(this);

                    UpdateTabs();
                    UpdateBarBackgroundColor();
                    UpdateBarTextColor();
                }

                if (bottomBarPage.CurrentPage != null)
                {
                    SwitchContent(bottomBarPage.CurrentPage);
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(TabbedPage.CurrentPage))
            {
                SwitchContent(Element.CurrentPage);
            }
            else if (e.PropertyName == NavigationPage.BarBackgroundColorProperty.PropertyName)
            {
                UpdateBarBackgroundColor();
            }
            else if (e.PropertyName == NavigationPage.BarTextColorProperty.PropertyName)
            {
                UpdateBarTextColor();
            }
        }

        protected virtual void SwitchContent(Page view)
        {
            Context.HideKeyboard(this);

            frameLayout.RemoveAllViews();

            if (view == null)
            {
                return;
            }

            if (Platform.GetRenderer(view) == null)
            {
                Platform.SetRenderer(view, Platform.CreateRenderer(view));
            }

            frameLayout.AddView(Platform.GetRenderer(view).ViewGroup);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var width = r - l;
            var height = b - t;

            var context = Context;

            bottomBar.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly), MeasureSpecFactory.MakeMeasureSpec(height, MeasureSpecMode.AtMost));
            var tabsHeight = Math.Min(height, Math.Max(bottomBar.MeasuredHeight, bottomBar.MinimumHeight));

            if (width > 0 && height > 0)
            {
                pageController.ContainerArea = new Rectangle(0, 0, context.FromPixels(width), context.FromPixels(frameLayout.Height));
                var internalChildren = pageController.InternalChildren;

                foreach (var el in internalChildren)
                {
                    var child = el as VisualElement;

                    if (child == null)
                    {
                        continue;
                    }

                    var renderer = Platform.GetRenderer(child);
                    var navigationRenderer = renderer as NavigationPageRenderer;
                    if (navigationRenderer != null)
                    {
                        // navigationRenderer.ContainerPadding = tabsHeight;
                    }
                }

                bottomBar.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly), MeasureSpecFactory.MakeMeasureSpec(tabsHeight, MeasureSpecMode.Exactly));
                bottomBar.Layout(0, 0, width, tabsHeight);
            }

            base.OnLayout(changed, l, t, r, b);
        }

        private void UpdateBarBackgroundColor()
        {
            if (disposed || bottomBar == null)
            {
                return;
            }

            bottomBar.SetBackgroundColor(Element.BarBackgroundColor.ToAndroid());
        }

        private void UpdateBarTextColor()
        {
            if (disposed || bottomBar == null)
            {
                return;
            }

            bottomBar.SetActiveTabColor(Element.BarTextColor.ToAndroid());
            // The problem SetActiveTabColor does only work in fiexed mode // haven't found yet how to set text color for tab items on_bottomBar, doesn't seem to have a direct way
        }

        private void UpdateTabs()
        {
            // create tab items
            SetTabItems();

            // set tab colors
            SetTabColors();
        }

        private void SetTabItems()
        {
            var tabs = Element.Children.Select(page => {
                var tabIconId = ResourceManagerEx.IdFromTitle(page.Icon, ResourceManager.DrawableClass);
                return new BottomBarTab(tabIconId, page.Title);
            }).ToArray();

            bottomBar.SetItems(tabs);
        }

        private void SetTabColors()
        {
            for (var i = 0; i < Element.Children.Count; ++i)
            {
                var page = Element.Children[i];

                var tabColor = page.GetTabColor();

                if (tabColor != null)
                {
                    bottomBar.MapColorForTab(i, tabColor.Value.ToAndroid());
                }
            }
        }
    }
}