using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Model;
using Windows.Graphics.Printing;

namespace Windows_Programming.Helpers
{
    public class ActivityTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DiscoverFormat { get; set; }
        public DataTemplate TransportFormat { get; set; }
        public DataTemplate LodgingFormat { get; set; }
        public DataTemplate ExtendFormat { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is Extend)
                return ExtendFormat;
            else if (item is Transport)
            {
                return TransportFormat;
            }
            else if (item is Lodging)
            {
                return LodgingFormat;
            }
            else if (item is Activity)
            {
                return DiscoverFormat;
            }
           return base.SelectTemplateCore(item);
        }
    }
}
