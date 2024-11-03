using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_Programming.Converter
{
    public class IntToVnCurrencyConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int number = (int)value;
            CultureInfo vietnameseCulture = new CultureInfo("vi-VN");
            string formattedAmount = number.ToString("C0", vietnameseCulture);
            return formattedAmount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
