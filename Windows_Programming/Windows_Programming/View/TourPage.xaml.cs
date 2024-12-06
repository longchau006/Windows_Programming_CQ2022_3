using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows_Programming.Model;
using Windows_Programming.ViewModel;

namespace Windows_Programming.View
{
    public sealed partial class TourPage : Page
    {
        private Tour Tour { get; set; }
        public TourPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Tour tour)
            {
                LoadTour(tour);
            }
        }

        private void LoadTour(Tour tour)
        {
            TourDetail.DataContext = tour;
            Tour = tour;
        }

        private void AddToPlanClick(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Tour tour)
            {
                Frame.Navigate(typeof(AddTourToPlanPage), tour);

            }
        }
    }
}
