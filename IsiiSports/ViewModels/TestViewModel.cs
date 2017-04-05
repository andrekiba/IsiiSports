using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IsiiSports
{
	public class TestViewModel : INotifyPropertyChanged
	{
		private string testProp;
		public string TestProp
		{
			get { return testProp; }
			set
			{
				if (value != testProp)
				{
					testProp = value;
					OnPropertyChanged();
				}
			}
		}

		public TestViewModel()
		{
			//PropertyChanged += (sender, e) => { };
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			var handler = PropertyChanged;

			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
