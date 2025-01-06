using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows_Programming.Model;
using WinRT.Interop;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows_Programming.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UpdateBlogPage : Page
    {
        private Blog thisBlog { get; set; }
        public string imagePath { get; set; }

        public UpdateBlogPage()
        {
            this.InitializeComponent();
            thisBlog = new Blog();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Blog blog)
            {
                thisBlog = blog;
                LoadBlog(blog);
            }
        }

        private void LoadBlog(Blog blog)
        {
            Title_TextBox.Text = blog.Title;
            Content_TextBox.Text = blog.Content;
            byte[] imageBytes = System.Convert.FromBase64String(blog.Image);
            using (var stream = new MemoryStream(imageBytes))
            {
                var bitmapImage = new BitmapImage();
                stream.Position = 0;
                bitmapImage.SetSource(stream.AsRandomAccessStream());
                image.Source = bitmapImage;
            }
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Title_TextBox.Text) || string.IsNullOrEmpty(Content_TextBox.Text))
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Update Blog",
                    Content = "No field can be empty",
                    CloseButtonText = "Ok",
                    XamlRoot = this.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
                return;
            }

            if (Title_TextBox.Text != thisBlog.Title)
            {
                thisBlog.Title = Title_TextBox.Text;
            }
            else
            {
                thisBlog.Title = null;
            }

            if (Content_TextBox.Text != thisBlog.Content)
            {
                thisBlog.Content = Content_TextBox.Text;
            }
            else
            {
                thisBlog.Content = null;
            }

            if (imagePath == null)
            {
                thisBlog.Image = null;
            }
            else
            {
                thisBlog.Image = imagePath;
            }

            BlogViewModel viewModel = new BlogViewModel();
            await viewModel.UpdateBlog(thisBlog);

            ContentDialog contentDialog = new ContentDialog
            {
                Title = "Update Blog",
                Content = "Blog updated successfully!",
                CloseButtonText = "Ok",
                XamlRoot = this.Content.XamlRoot
            };
            await contentDialog.ShowAsync();
            Frame.GoBack();
        }

        private async void ChangeImage_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();
            Window w = new();
            var hWnd = WindowNative.GetWindowHandle(w);
            InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".jpg");
            StorageFile file = await openPicker.PickSingleFileAsync();
            w.Close();

            if (file != null)
            {
                imagePath = file.Path;
                ChangeImage.Content = file.Name;
                image.Source = new BitmapImage(new Uri(file.Path));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
