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

		protected async Task DoAction(Func<Task> action)
		{
			Exception exception = null;
			try
			{
				IsBusy = true;
				await action();
				IsBusy = false;
			}
			catch (Exception ex)
			{
				exception = ex;
				IsBusy = false;
			}

			if (exception != null)
				await CoreMethods.DisplayAlert("Ops", "Error", "OK");
		}
    }
}
