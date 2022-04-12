using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SkyrimAnimationChecker
{
    public class RadioButtonSelector : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int param;
            try { param = System.Convert.ToInt32(parameter); }
            catch { throw new ArgumentException("Parameter type is not int."); }
            if (value is int)
            {
                if ((int)value == param) return true;
                else return false;
            }
            else throw new ArgumentException("Value type is not int.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int param;
            try { param = System.Convert.ToInt32(parameter); }
            catch { throw new ArgumentException("Parameter type is not int."); }
            if (value is bool)
            {
                if ((bool)value == true) return param;
                else return -1;
            }
            else throw new ArgumentException("Value type is not bool.");
        }
    }
    public class WeightSelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int param;
            try { param = System.Convert.ToInt32(parameter); }
            catch { throw new ArgumentException("Parameter type is not int."); }
            //if (param != 0 && param != 1 && param != 2) throw new ArgumentException("Parameter is not set to 0, 1 or 2.");
            if (value is int)
            {
                if ((int)value == param) return true;
                else return false;
            }
            else throw new ArgumentException("Value type is not int.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int param;
            try { param = System.Convert.ToInt32(parameter); }
            catch { throw new ArgumentException("Parameter type is not int."); }
            //if (param != 0 && param != 1 && param != 2) throw new ArgumentException("Parameter is not set to 0, 1 or 2.");
            if (value is bool)
            {
                if ((bool)value == true) return param;
                else return -1;
            }
            else throw new ArgumentException("Value type is not bool.");
        }
    }
}
