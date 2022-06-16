using System;
using System.Globalization;
using System.Windows;
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
            if (value is int val)
            {
                if (val == param) return true;
                else return false;
            }
            else throw new ArgumentException("Value type is not int.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int param;
            try { param = System.Convert.ToInt32(parameter); }
            catch { throw new ArgumentException("Parameter type is not int."); }
            if (value is bool val)
            {
                if (val == true) return param;
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
            if (value is int val)
            {
                if (val == param) return true;
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
            if (value is bool val)
            {
                if (val == true) return param;
                else return -1;
            }
            else throw new ArgumentException("Value type is not bool.");
        }
    }


    public class StringDoubleConverter : IValueConverter
    {
        private double OldValue = 0;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double val)
            {
                OldValue = val;
                return value.ToString() ?? string.Empty;
            }
            throw new ArgumentException("value is not double");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double val = System.Convert.ToDouble(value);
                OldValue = val;
                return val;
            }
            catch (FormatException) { }
            catch (InvalidCastException) { }
            return OldValue;
        }
    }

    public class CollectiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //M.D("HERE");
                //M.D(value);
                if (string.IsNullOrWhiteSpace((string)value)) return Visibility.Visible;
                foreach (string s in ((string)parameter).Split(','))
                {
                    if ((string)value == s) return Visibility.Visible;
                }
            }
            catch { throw new ArgumentException("Invalid value or parameter"); }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
