using Microsoft.UI.Xaml.Data;
using System;

namespace Windows_Programming.Converters
{
    public class DeletedDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null ? $"Deleted at {value}" : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
