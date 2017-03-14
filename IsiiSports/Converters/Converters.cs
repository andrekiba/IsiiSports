using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using IsiiSports.Helpers;
using Xamarin.Forms;

namespace IsiiSports.Converters
{
    public class NullIntValueConverter : IValueConverter
    {
        public static NullIntValueConverter Instance = new NullIntValueConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? string.Empty : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return null;

            int i;
            if (int.TryParse((string)value, out i))
                return i;

            return null;
        }
    }

    public class InverseBoolConverter : IValueConverter
    {
        public static InverseBoolConverter Instance = new InverseBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    public class IsNotNullToBoolConverter : IValueConverter
    {
        public static IsNotNullToBoolConverter Instance = new IsNotNullToBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }
    }

    public class IsNullToBoolConverter : IValueConverter
    {
        public static IsNullToBoolConverter Instance = new IsNullToBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }
    }

    public class IsEmptyConverter : IValueConverter
    {
        public static IsEmptyConverter Instance = new IsEmptyConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = value as IList;

            return list?.Count > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = value as IList;

            return list?.Count > 0;
        }
    }

    public class NameToInitialsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
                return "<>";
            var words = value.ToString().Split(' ').Take(2);
            return words.Aggregate("", (current, word) => current + word[0]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RandomColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var r = RandomGenerator.Instance;
            var rnd = r.GetNext(1, 4);

            switch (rnd)
            {
                case 1:
                    return Color.FromHex("#27ae60");
                case 2:
                    return Color.FromHex("#2980b9");
                case 3:
                    return Color.FromHex("#8e44ad");
                case 4:
                    return Color.FromHex("#c0392b");
                default:
                    return Color.FromHex("#f39c12");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
