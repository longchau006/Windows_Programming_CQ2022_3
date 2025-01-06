using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows_Programming.Model;
using Windows_Programming.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyBlogsPage : Page
    {
        public MyBlogsPage()
        {
            this.InitializeComponent();
            LoadBlog();
        }

        private async void LoadBlog()
        {
            BlogViewModel blogViewModel = new BlogViewModel();
            await blogViewModel.GetOwnBlog(MainWindow.MyAccount.Id);
            if (blogViewModel.OwnBlogs.Count == 0)
            {
                NoBlogTextBlock.Visibility = Visibility.Visible;
                BlogListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoBlogTextBlock.Visibility = Visibility.Collapsed;
                BlogListView.Visibility = Visibility.Visible;
                BlogListView.DataContext = blogViewModel;
            }
            
        }


        private void OnBlog_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Blog blog)
            {
                // Navigate to BlogDetailPage with the selected blog's ID
                Frame.Navigate(typeof(BlogPage), blog.Id);
            }
        }
    }
}
