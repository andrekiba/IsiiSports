using System;
using System.Threading.Tasks;
using FreshMvvm;
using IsiiSports.Interfaces;
using PropertyChanged;
using Xamarin.Forms;

namespace IsiiSports.ViewModels
{
    [ImplementPropertyChanged]
    public class BaseViewModel : FreshBasePageModel
    {
		protected IAzureService AzureService { get; } = Application.Current != null ? FreshIOC.Container.Resolve<IAzureService>() : null;

        public bool IsBusy { get; set; }

        public bool IsNotBusy => !IsBusy;

        public string LoadingMessage { get; set; }
    }
}
