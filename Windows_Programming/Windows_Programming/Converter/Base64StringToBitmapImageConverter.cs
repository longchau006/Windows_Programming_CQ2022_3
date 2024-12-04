using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;

namespace Windows_Programming.Converter
{
    public partial class Base64StringToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string base64String)
            {
                try
                {
                    byte[] imageBytes = System.Convert.FromBase64String(base64String);
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        var bitmapImage = new BitmapImage();
                        stream.Position = 0; // Đặt lại vị trí stream để đảm bảo đọc từ đầu
                        bitmapImage.SetSource(stream.AsRandomAccessStream());
                        return bitmapImage;
                    }
                }
                catch
                {
                    // Handle lỗi nếu chuỗi base64 không hợp lệ
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
