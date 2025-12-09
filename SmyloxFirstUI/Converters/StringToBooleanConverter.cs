using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SmyloxFirstUI.Converters
{
    public class StringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasValue = !string.IsNullOrEmpty(value as string);

            // If target type is Visibility, return Visibility enum
            if (targetType == typeof(Visibility))
            {
                return hasValue ? Visibility.Visible : Visibility.Collapsed;
            }

            // Otherwise return boolean
            return hasValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
