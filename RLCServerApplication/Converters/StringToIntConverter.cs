using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RLCServerApplication.Converters
{
    public class UIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint result = (uint)value;
            return result.ToString();
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is string)) {
                return 0;
            }
            if(uint.TryParse((value as string), out uint result)) {
                return result;
            }
            return 0;
        }
    }
}
