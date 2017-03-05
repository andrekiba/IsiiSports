using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace IsiiSports.Droid.Utils
{
    public class PageController : IPageController
    {
        private readonly ReflectedProxy<Page> proxy;

        public static IPageController Create(Page page)
        {
            return new PageController(page);
        }

        private PageController(Page page)
        {
            proxy = new ReflectedProxy<Page>(page);
        }

        public Rectangle ContainerArea
        {
            get
            {
                return proxy.GetPropertyValue<Rectangle>();
            }

            set
            {
                proxy.SetPropertyValue(value);
            }
        }

        public bool IgnoresContainerArea
        {
            get
            {
                return proxy.GetPropertyValue<bool>();
            }

            set
            {
                proxy.SetPropertyValue(value);
            }
        }

        public ObservableCollection<Element> InternalChildren
        {
            get
            {
                return proxy.GetPropertyValue<ObservableCollection<Element>>();
            }

            set
            {
                proxy.SetPropertyValue(value);
            }
        }

        public void SendAppearing()
        {
            proxy.Call();
        }

        public void SendDisappearing()
        {
            proxy.Call();
        }
    }
}