using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows_Programming.ViewModel;
using Windows_Programming.Model;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Devices.PointOfService;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>  
    /// An empty page that can be used on its own or navigated to within a Frame.  
    /// </summary>  
    public sealed partial class BlogListPage : Page
    {
        public BlogListPage()
        {
            this.InitializeComponent();
            var viewModel = new BlogViewModel();
            viewModel.ViewBlog();
            BlogListView.DataContext = viewModel;

            var viewModel2 = new BlogViewModel();
            viewModel2.ViewLastestBlog();
            BlogFlipView.DataContext = viewModel2;
        }

        private void OnBlogTapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Blog blog)
            {
                // Navigate to BlogDetailPage with the selected blog's ID
                Frame.Navigate(typeof(BlogPage), blog.Id);

            }
        }

        private void FlipView_Tag(object sender, TappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Blog blog)
            {
                // Navigate to BlogDetailPage with the selected blog's ID
                Frame.Navigate(typeof(BlogPage), blog.Id);
            }

        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateBlogPage));
        }
    }
}
