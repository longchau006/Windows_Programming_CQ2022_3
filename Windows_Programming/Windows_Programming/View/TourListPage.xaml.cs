using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows_Programming.Model;
using Windows_Programming.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TourListPage : Page
    {
        public TourListPage()
        {
            this.InitializeComponent();
            //addComponent();
            loadTour();
        }

        private async void loadTour()
        {
            var viewModel = new TourViewModel();
            await viewModel.GetAllTour();
            TourListView.DataContext = viewModel;
        }

       /* private async void addComponent()
        {
            var viewModel = new TourViewModel();
            await viewModel.AddTour();
        }*/
        private void OnBlogTapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Tour tour)
            {
                Frame.Navigate(typeof(TourPage), tour);

            }
        }
    }
}
