using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace RLCServerApplication.Converters
{
    public class ValidationErrorsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable<ValidationError>) {
                return string.Join(Environment.NewLine, ((IEnumerable)value).OfType<ValidationError>().Select(x => x.ErrorContent).ToArray());
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
