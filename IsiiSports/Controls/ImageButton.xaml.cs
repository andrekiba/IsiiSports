using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace IsiiSports.Controls
{
    public partial class ImageButton : ContentView
    {
        public event EventHandler Clicked;

        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(Button));
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(Button));
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly BindableProperty ButtonBackgroundColorProperty = BindableProperty.Create("ButtonBackgroundColor", typeof(Color), typeof(ImageButton), Color.Transparent);
        public Color ButtonBackgroundColor
        {
            get { return (Color)GetValue(ButtonBackgroundColorProperty); }
            set { SetValue(ButtonBackgroundColorProperty, value); }
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(ImageButton));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(FileImageSource), typeof(ImageButton));
        public FileImageSource Source
        {
            get { return (FileImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

		public static readonly BindableProperty ImageHeightRequestProperty = BindableProperty.Create("ImageHeightRequest", typeof(int), typeof(ImageButton));
		public int ImageHeightRequest
		{
			get { return (int)GetValue(ImageHeightRequestProperty); }
			set { SetValue(ImageHeightRequestProperty, value); }
		}

		public static readonly BindableProperty ImageWidthRequestProperty = BindableProperty.Create("ImageWidthRequest", typeof(int), typeof(ImageButton));
		public int ImageWidthRequest
		{
			get { return (int)GetValue(ImageWidthRequestProperty); }
			set { SetValue(ImageWidthRequestProperty, value); }
		}

        public ImageButton()
        {
            InitializeComponent();
            Root.BindingContext = this;
        }

        private async void HandleClick(object sender, EventArgs e)
        {
            Clicked?.Invoke(this, e);

            await Root.ScaleTo(1.2, 100);
            await Root.ScaleTo(1, 100);
        }
    }
}
