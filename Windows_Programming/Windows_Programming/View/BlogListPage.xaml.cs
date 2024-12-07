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
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>  
    /// An empty page that can be used on its own or navigated to within a Frame.  
    /// </summary>  
    public sealed partial class BlogListPage : Page
    {
        private ContentDialog loadingDialog;
        public BlogViewModel viewModel { get; set; }
        public BlogListPage()
        {
            this.InitializeComponent();
            this.Loaded += BlogListPage_Loaded;
        }

        private async void BlogListPage_Loaded(object sender, RoutedEventArgs e)
        {
            CreateLoadingDialog();
            await InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            var loadingTask = loadingDialog.ShowAsync();
            viewModel = new BlogViewModel();
            await viewModel.GetAllBlog();
            BlogListView.DataContext = viewModel;

            viewModel.GetLastestBlog();
            BlogFlipView.DataContext = viewModel;
            loadingDialog.Hide();
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

        private void MyBlog_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MyBlogsPage));
        }
        private void mySearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            viewModel.searchBlog(mySearchBox.Text);
            BlogListView.DataContext = null;
            BlogListView.DataContext = viewModel;
        }

        private void mySearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
           /* viewModel.searchBlog(mySearchBox.Text);
            BlogListView.DataContext = null;
            BlogListView.DataContext = viewModel;*/
        }

        private void CreateLoadingDialog()
        {
            StackPanel dialogContent = new StackPanel
            {
                Spacing = 10,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            ProgressRing progressRing = new ProgressRing
            {
                IsActive = true,
                Width = 50,
                Height = 50
            };

            TextBlock messageText = new TextBlock
            {
                Text = "Please wait...",
                HorizontalAlignment = HorizontalAlignment.Center
            };

            dialogContent.Children.Add(progressRing);
            dialogContent.Children.Add(messageText);

            loadingDialog = new ContentDialog
            {
                Content = dialogContent,
                IsPrimaryButtonEnabled = false,
                IsSecondaryButtonEnabled = false,
                XamlRoot = this.XamlRoot  // Set the XamlRoot from the current page
            };
        }

    }
}
