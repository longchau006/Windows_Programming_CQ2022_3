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
using Microsoft.UI.Xaml.Media.Imaging;
using Windows_Programming.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlogPage : Page
    {
        private BlogViewModel viewModel = new BlogViewModel();
        public BlogPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int blogId)
            {
                LoadBlog(blogId);
            }
        }

        private void LoadBlog(int blogId)
        {
            var blog = viewModel.GetBlogById(blogId);
            if (blog != null)
            {
                BlogImage.Source = new BitmapImage(new Uri(blog.Image));
                BlogTitle.Text = blog.Title;
                BlogContent.Text = blog.Content;
            }
        }
    }
}
