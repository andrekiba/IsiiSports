using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreshMvvm;
using PropertyChanged;

namespace IsiiSports.ViewModels
{
    [ImplementPropertyChanged]
    public class BaseViewModel : FreshBasePageModel
    {
        public bool IsBusy { get; set; }

        public bool IsNotBusy => !IsBusy;
    }
}
