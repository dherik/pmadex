using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PmaDex.Converter
{
    public class ListLengthVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            bool invertOuput = false;
            if (parameter != null) {
                bool.TryParse((string)parameter, out invertOuput);
            }

            bool hasElements = ((IEnumerable<object>)value).Any();

            if (hasElements)
            {
                return !invertOuput ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return !invertOuput ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
