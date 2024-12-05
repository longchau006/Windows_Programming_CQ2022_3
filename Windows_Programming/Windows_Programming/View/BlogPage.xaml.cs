using System;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows_Programming.ViewModel;
using Windows_Programming.Model;
using Microsoft.UI.Xaml;

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
        private Blog blog { get; set; }

        public BlogPage()
        {
            this.InitializeComponent();

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is string blogId)
            {
                LoadBlog(blogId);
            }
        }

        private async void LoadBlog(string blogId)
        {
            blog = await viewModel.GetBlogById(blogId);
            if (blog != null)
            {
                BlogTitle.Text = blog.Title;
                BlogContent.Text = blog.Content;
                BlogPublishDate.Text = blog.PublishDate.ToString();

                byte[] imageBytes = Convert.FromBase64String(blog.Image);
                using (var stream = new MemoryStream(imageBytes))
                {
                    var bitmapImage = new BitmapImage();
                    stream.Position = 0;
                    bitmapImage.SetSource(stream.AsRandomAccessStream());
                    BlogImage.Source = bitmapImage;
                }
                BlogViewModel blogViewModel = new BlogViewModel();
                var author = await blogViewModel.GetAccount(blog.Author);
                BlogAuthor.Text = author.Fullname;
            }
            if (blog.Author == MainWindow.MyAccount.Id)
            {
                OwnBlog_CommandBar.Visibility = Visibility.Visible;
            }
        }

        private async void DeleteBlog_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Delete Blog",
                Content = "Are you sure you want to delete this blog?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = this.Content.XamlRoot
            };
            deleteDialog.PrimaryButtonClick += async (_sender, _e) =>
            {
                viewModel.DeleteBlog(blog.Id);
                Frame.GoBack();
            };
            await deleteDialog.ShowAsync();
        }

        private void EditBlog_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UpdateBlogPage), blog);
        }
    }
}
