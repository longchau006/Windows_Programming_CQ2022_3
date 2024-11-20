using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_Programming.Helpers
{
    public class DateTimeToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.TimeOfDay; // Lấy phần thời gian từ DateTime
            }
            return default(TimeSpan);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan timeSpan)
            {
                return DateTime.Today.Add(timeSpan); // Tạo DateTime từ TimeSpan với ngày hiện tại
            }
            return null;
        }
    }
}
