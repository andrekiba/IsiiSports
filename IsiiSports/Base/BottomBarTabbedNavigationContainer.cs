using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BottomBar.XamarinForms;
using FreshMvvm;
using Xamarin.Forms;

namespace IsiiSports.Base
{
    public class BottomBarTabbedNavigationContainer : BottomBarPage, IFreshNavigationService
    {
        private readonly List<Page> tabs = new List<Page>();

        public IEnumerable<Page> TabbedPages => tabs;

        public string NavigationServiceName { get; }

        public BottomBarTabbedNavigationContainer() : this("DefaultNavigationServiceName") { }

        public BottomBarTabbedNavigationContainer(string navigationServiceName)
        {
            NavigationServiceName = navigationServiceName;
            RegisterNavigation();
        }

        protected void RegisterNavigation()
        {
            FreshIOC.Container.Register((IFreshNavigationService)this, NavigationServiceName);
        }

        public virtual Page AddTab<T>(string title, string icon, object data = null) where T : FreshBasePageModel
        {
            var page = FreshPageModelResolver.ResolvePageModel<T>(data);
            page.GetModel().CurrentNavigationServiceName = NavigationServiceName;
            tabs.Add(page);
            var containerPageSafe = CreateContainerPageSafe(page);
            containerPageSafe.Title = title;
            if (!string.IsNullOrWhiteSpace(icon))
                containerPageSafe.Icon = icon;
            Children.Add(containerPageSafe);
            return containerPageSafe;
        }

        internal Page CreateContainerPageSafe(Page page)
        {
            if (page is NavigationPage || page is MasterDetailPage || page is TabbedPage)
                return page;
            return CreateContainerPage(page);
        }

        protected virtual Page CreateContainerPage(Page page)
        {
            return new NavigationPage(page);
        }

        public Task PushPage(Page page, FreshBasePageModel model, bool modal = false, bool animate = true)
        {
            return modal ? CurrentPage.Navigation.PushModalAsync(CreateContainerPageSafe(page)) : CurrentPage.Navigation.PushAsync(page);
        }

        public Task PopPage(bool modal = false, bool animate = true)
        {
            return modal ? CurrentPage.Navigation.PopModalAsync(animate) : CurrentPage.Navigation.PopAsync(animate);
        }

        public Task PopToRoot(bool animate = true)
        {
            return CurrentPage.Navigation.PopToRootAsync(animate);
        }

        public void NotifyChildrenPageWasPopped()
        {
            foreach (var child in Children)
            {
                if (child is NavigationPage)
                    ((NavigationPage)child).NotifyAllChildrenPopped();
            }
        }

        public Task<FreshBasePageModel> SwitchSelectedRootPageModel<T>() where T : FreshBasePageModel
        {
            var index = tabs.FindIndex(o => o.GetModel().GetType().FullName == typeof(T).FullName);
            if (index > -1)
            {
                CurrentPage = Children[index];
                var page = CurrentPage.Navigation.NavigationStack.LastOrDefault();
                if (page != null)
                    return Task.FromResult(page.GetModel());
            }
            return null;
        }
    }
}
