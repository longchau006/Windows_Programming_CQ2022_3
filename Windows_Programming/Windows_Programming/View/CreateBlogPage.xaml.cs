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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateBlogPage : Page
    {
        public string imagePath { get; set; }

        public CreateBlogPage()
        {
            this.InitializeComponent();
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            Blog blog = new Blog();
            blog.Title = Title_TextBox.Text;
            blog.Content = Content_TextBox.Text;
            blog.Image = $"ms-appx://{imagePath}";
            blog.PublishDate = DateTime.Now;
            AccountViewModel accountViewModel = new AccountViewModel();
            accountViewModel.getInformation();
            blog.Author = accountViewModel.User.Username;
            BlogViewModel blogViewModel = new BlogViewModel();
            blogViewModel.ViewBlog();
            blog.Id = blogViewModel.blogInBlogList.Count + 1;
            blogViewModel.AddBlog(blog);
            Frame.Navigate(typeof(BlogListPage));
        }

        private async void ChooseImageClick(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();
            Window w = new();
            var hWnd = WindowNative.GetWindowHandle(w);
            InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");
            StorageFile file = await openPicker.PickSingleFileAsync();
            w.Close();

            if (file != null)
            {
                imagePath = file.Path;
                Image.Content = imagePath;
            }
        }
    }
}
