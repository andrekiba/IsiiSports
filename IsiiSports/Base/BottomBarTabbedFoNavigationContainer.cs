using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreshMvvm;
using IsiiSports.Controls;
using Xamarin.Forms;

namespace IsiiSports.Base
{
	public class BottomBarTabbedFoNavigationContainer : NavigationPage, IFreshNavigationService
	{
	    public BottomBarPage FirstTabbedPage { get; }

	    private readonly List<Page> tabs = new List<Page>();

		public IEnumerable<Page> TabbedPages => tabs;

	    public BottomBarTabbedFoNavigationContainer(string titleOfFirstTab) : this(titleOfFirstTab, Constants.DefaultNavigationServiceName)
        {
		}

		public BottomBarTabbedFoNavigationContainer(string titleOfFirstTab, string navigationServiceName) : base(new BottomBarPage())
        {
			NavigationServiceName = navigationServiceName;
			RegisterNavigation();
			FirstTabbedPage = (BottomBarPage)CurrentPage;
			FirstTabbedPage.Title = titleOfFirstTab;
		}

		protected void RegisterNavigation()
		{
			FreshIOC.Container.Register<IFreshNavigationService>(this, NavigationServiceName);
		}

		public virtual Page AddTab<T>(string title, string icon, object data = null) where T : FreshBasePageModel
		{
			var page = FreshPageModelResolver.ResolvePageModel<T>(data);
			page.GetModel().CurrentNavigationServiceName = NavigationServiceName;
			tabs.Add(page);
			var container = CreateContainerPageSafe(page);
			container.Title = title;
			if (!string.IsNullOrWhiteSpace(icon))
				container.Icon = icon;
			FirstTabbedPage.Children.Add(container);
			return container;
		}

		internal Page CreateContainerPageSafe(Page page)
		{
			if (page is NavigationPage || page is MasterDetailPage || page is TabbedPage)
				return page;

			return CreateContainerPage(page);
		}

		protected virtual Page CreateContainerPage(Page page)
		{
			return page;
		}

		public Task PushPage(Page page, FreshBasePageModel model, bool modal = false, bool animate = true)
		{
		    return modal ? Navigation.PushModalAsync(CreateContainerPageSafe(page)) : Navigation.PushAsync(page);
		}

		public Task PopPage(bool modal = false, bool animate = true)
		{
		    return modal ? Navigation.PopModalAsync(animate) : Navigation.PopAsync(animate);
		}

		public Task PopToRoot(bool animate = true)
		{
			return Navigation.PopToRootAsync(animate);
		}

		public string NavigationServiceName { get; private set; }

		public void NotifyChildrenPageWasPopped()
		{
			foreach (var page in FirstTabbedPage.Children)
			{
				if (page is NavigationPage)
					((NavigationPage)page).NotifyAllChildrenPopped();
			}
		}

		public Task<FreshBasePageModel> SwitchSelectedRootPageModel<T>() where T : FreshBasePageModel
		{
			if (CurrentPage == FirstTabbedPage)
			{
				var page = tabs.FindIndex(o => o.GetModel().GetType().FullName == typeof(T).FullName);
				if (page > -1)
				{
					FirstTabbedPage.CurrentPage = FirstTabbedPage.Children[page];
					return Task.FromResult(FirstTabbedPage.CurrentPage.GetModel());
				}
			}
			else
			{
				throw new Exception("Cannot switch tabs when the tab screen is not visible");
			}

			return null;
		}
	}
}
