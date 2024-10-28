using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows_Programming.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TourPage : Page
    {
        private TourViewModel viewModel = new TourViewModel();
        public TourPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int tourId)
            {
                LoadTour(tourId);
            }
        }

        private void LoadTour(int tourId)
        {
            var tour = viewModel.GetTourById(tourId);
            if (tour != null)
            {
                TourImane_TextBlock.Source = new BitmapImage(new Uri(tour.Image));
                TourName_TextBlock.Text = tour.Name;
                TourDescript_TextBlock.Text = tour.Description;
                TourSchedule_TextBlock.Text = tour.Schedule;
            }
        }
    }

}
