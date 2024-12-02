using System;
using System.IO;
using Microsoft.UI.Xaml.Controls;
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
            if (e.Parameter is string blogId)
            {
                LoadBlog(blogId);
            }
        }

        private async void LoadBlog(string blogId)
        {
            var blog = await viewModel.GetBlogById(blogId);
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
        }
    }
}
