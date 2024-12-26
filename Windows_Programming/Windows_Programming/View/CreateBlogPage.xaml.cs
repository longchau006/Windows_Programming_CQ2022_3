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
using Windows.Storage.Pickers;
using WinRT.Interop;
using Windows.Storage;
using Windows_Programming.Model;
using Windows_Programming.ViewModel;
using System.Buffers.Text;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateBlogPage : Page
    {
        private ContentDialog loadingDialog;

        public string imagePath { get; set; }
        public byte[] imageBytes { get; set; }
        private string url_text { get; set; }

        public CreateBlogPage()
        {
            this.InitializeComponent();
            imageBytes = null;
            url_text = ImageURL.Text;
        }

        private async void SubmitClick(object sender, RoutedEventArgs e)
        {

            CreateLoadingDialog();
            Blog blog = new Blog();
            blog.Title = Title_TextBox.Text;
            blog.Content = Content_TextBox.Text;
            blog.Image = imagePath;
            blog.PublishDate = DateTime.Now;
            blog.Author = MainWindow.MyAccount.Id;
            BlogViewModel blogViewModel = new BlogViewModel();
            try
            {
                var loadingTask = loadingDialog.ShowAsync();
                await blogViewModel.AddBlog(blog);
                loadingDialog.Hide();
            }
            catch (Exception ex)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
            }
            ContentDialog contentDialog = new ContentDialog
            {
                Title = "Success",
                Content = "Blog created successfully",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await contentDialog.ShowAsync();
            Frame.Navigate(typeof(BlogListPage));
        }

        private async void ChooseImageClick(object sender, RoutedEventArgs e)
        {
            if (imageBytes != null)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "You can't choose image from URL and local file at the same time",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }
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
                Image.Content = file.Name;
            }
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

        private async void GetImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImageURL.Text == url_text)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "You must enter image url",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            if (string.IsNullOrWhiteSpace(ImageURL.Text))
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "You must enter image url",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            if (imagePath != null)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "You can't choose image from URL and local file at the same time",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }
            if (imagePath != null)
            {
                return;
            }

            try {
                var httpClient = new System.Net.Http.HttpClient();
                imageBytes = await httpClient.GetByteArrayAsync(ImageURL.Text);

            } catch (Exception ex)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }
            url.Content = "Downloaded";
            url.IsEnabled = false;
            // get folder Assets of current app
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            StorageFile storageFile = await storageFolder.CreateFileAsync("image.jpg", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(storageFile, imageBytes);
            imagePath = storageFile.Path;
        }
    }
}
