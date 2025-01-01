using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_Programming.Model
{
    public class Message
    {
        public string Content { get; set; }
        public bool IsAI { get; set; }
        public DateTime TimeMessage { get; set; }
        public HorizontalAlignment Alignment => IsAI ? HorizontalAlignment.Left : HorizontalAlignment.Right;

        public string GetFormattedTimeMessage => TimeMessage.ToString("HH:mm - dd/MM/yyyy");
        public Microsoft.UI.Xaml.Media.SolidColorBrush BackgroundColor => IsAI ?
            new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightSkyBlue) :
            new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray);

    }
}
