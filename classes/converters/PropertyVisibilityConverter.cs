using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Company_Review.classes.converters
{
    public class PropertyVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible = (bool)value;
            if (visible)
                return "Visible";
            else
                return "Hidden";
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString().Contains("Visible"))
                return true;
            else
                return false;
        }

    }
}
